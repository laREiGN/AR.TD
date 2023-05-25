using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GameLoopManager : MonoBehaviour
{
    [HideInInspector] public Vector3[] WaypointPositions;
    [HideInInspector] public float[] WaypointDistances;

    [HideInInspector] public Queue<int> EnemiesToSummon;
    [HideInInspector] public Queue<Enemy> EnemiesToRemove;

    [HideInInspector] public List<Tower> activeTowers;

    public Transform WaypointContainer;
    public Transform EnemyContainer;

    public TowerManager TowerManager;
    // public TowerTargeting TowerTargeting;
    public EnemySpawner EnemySpawner;

    public bool GameIsOver;

    void OnEnable()
    {
        TowerManager = GetComponentInChildren<TowerManager>();
        // TowerTargeting = GetComponentInChildren<TowerTargeting>();
        EnemySpawner = GetComponentInChildren<EnemySpawner>();

        EnemiesToSummon = new Queue<int>();
        EnemiesToRemove = new Queue<Enemy>();
        EnemySpawner.Init();

        WaypointPositions = new Vector3[WaypointContainer.childCount];
        for (int i = 0; i < WaypointPositions.Length; i++)
        {
            WaypointPositions[i] = WaypointContainer.GetChild(i).position;
        }

        WaypointDistances = new float[WaypointPositions.Length - 1];
        for (int i = 0; i < WaypointDistances.Length; i++)
        {
            WaypointDistances[i] = Vector3.Distance(WaypointPositions[i], WaypointPositions[i + 1]);
        }

        TowerManager.Init();

        StartCoroutine(GameLoop());
        InvokeRepeating("SummonTest", 0f, 1f);
    }

    void SummonTest()
    {
        EnqueueEnemiesToSummon(1);
    }

    public void Reload()
    {
        WaypointPositions = new Vector3[WaypointContainer.childCount];
        for (int i = 0; i < WaypointPositions.Length; i++)
        {
            WaypointPositions[i] = WaypointContainer.GetChild(i).position;
        }
    }

    IEnumerator GameLoop()
    {
        while (!GameIsOver)
        {
            // SPAWN ENEMIES
            if (EnemiesToSummon.Count > 0)
            {
                for (int i = 0; i < EnemiesToSummon.Count; i++)
                {
                    EnemySpawner.SummonEnemy(EnemiesToSummon.Dequeue());
                }
            }

            // SPAWN TOWERS

            // MOVE ENEMIES
            NativeArray<Vector3> WaypointsToUse = new NativeArray<Vector3>(WaypointPositions, Allocator.TempJob);
            NativeArray<int> WaypointIndices = new NativeArray<int>(EnemySpawner.EnemiesInGame.Count, Allocator.TempJob);
            NativeArray<float> EnemySpeeds = new NativeArray<float>(EnemySpawner.EnemiesInGame.Count, Allocator.TempJob);
            TransformAccessArray EnemyAccess = new TransformAccessArray(EnemySpawner.EnemiesInGameAsTransforms.ToArray(), 2);

            for (int i = 0; i < EnemySpawner.EnemiesInGame.Count; i++)
            {
                EnemySpeeds[i] = EnemySpawner.EnemiesInGame[i].Speed;
                WaypointIndices[i] = EnemySpawner.EnemiesInGame[i].WaypointIndex;
            }

            MoveEnemiesJob MoveEnemiesJob = new MoveEnemiesJob
            {
                WaypointPositions = WaypointsToUse,
                WaypointIndex = WaypointIndices,
                EnemySpeed = EnemySpeeds,
                deltaTime = Time.deltaTime
            };

            JobHandle MoveJobHandle = MoveEnemiesJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();

            for (int i = 0; i < EnemySpawner.EnemiesInGame.Count; i++)
            {
                EnemySpawner.EnemiesInGame[i].WaypointIndex = WaypointIndices[i];
                if (EnemySpawner.EnemiesInGame[i].WaypointIndex == WaypointPositions.Length)
                {
                    EnqueueEnemyToRemove(EnemySpawner.EnemiesInGame[i]);
                }
            }


            WaypointsToUse.Dispose();
            WaypointIndices.Dispose();
            EnemySpeeds.Dispose();
            EnemyAccess.Dispose();

            // TICK TOWERS
            // foreach(Tower tower in activeTowers)
            // {
            //     tower.currentTarget = TowerTargeting.GetTarget(tower, TowerTargeting.TargetType.First);
            //     tower.Tick();
            // }

            // APPLY EFFECTS

            // DAMAGE ENEMIES

            // REMOVE ENEMIES
            if (EnemiesToRemove.Count > 0)
            {
                for (int i = 0; i < EnemiesToRemove.Count; i++)
                {
                    EnemySpawner.RemoveEnemy(EnemiesToRemove.Dequeue());
                }
            }

            // REMOVE TOWERS

            yield return null;
        }
    }

    public void EnqueueEnemiesToSummon(int EnemyID)
    {
        EnemiesToSummon.Enqueue(EnemyID);
    }

    public void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> WaypointPositions;

    [NativeDisableParallelForRestriction]
    public NativeArray<int> WaypointIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;

    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if (WaypointIndex[index] < WaypointPositions.Length)
        {
            Vector3 TargetPosition = WaypointPositions[WaypointIndex[index]];
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, (EnemySpeed[index] / 100) * deltaTime);

            if (transform.position == TargetPosition)
            {
                WaypointIndex[index]++;
            }
        }
    }
}
