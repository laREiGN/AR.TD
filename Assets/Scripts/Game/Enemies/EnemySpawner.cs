using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [HideInInspector] public List<Enemy> EnemiesInGame;
    [HideInInspector] public List<Transform> EnemiesInGameAsTransforms;
    [HideInInspector] public Dictionary<int, GameObject> EnemyPrefabs;
    [HideInInspector] public Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    private GameLoopManager GameLoopManager;
    private bool IsInitialized;

    private void OnEnable()
    {
        GameLoopManager = GetComponentInParent<GameLoopManager>();
    }
    public void Init()
    {
        if (!IsInitialized)
        {
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            EnemiesInGame = new List<Enemy>();
            EnemiesInGameAsTransforms = new List<Transform>();

            EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");

            foreach (EnemySummonData enemy in Enemies)
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
            }

            IsInitialized = true;
        }
        else
        {
            Debug.Log("ENEMYSPAWNER: THIS CLASS IS ALREADY INITIALIZED");
        }
    }

    public Enemy SummonEnemy(int EnemyID)
    {
        Enemy SummonedEnemy;
        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<Enemy> ReferenceQueue = EnemyObjectPools[EnemyID];
            if (ReferenceQueue.Count > 0)
            {
                // DEQUEUE ENEMY AND INITIALIZE
                SummonedEnemy = ReferenceQueue.Dequeue();
                SummonedEnemy.Init();

                SummonedEnemy.gameObject.SetActive(true);
            } 
            else
            {
                // INSTANTIATE NEW INSTANCE OF ENEMY AND INITIALIZE
                GameObject NewEnemy = Instantiate(EnemyPrefabs[EnemyID], GameLoopManager.WaypointPositions[0], Quaternion.identity, GameLoopManager.EnemyContainer);
                SummonedEnemy = NewEnemy.GetComponent<Enemy>();
                SummonedEnemy.Init();
            }
        }
        else
        {
            Debug.Log($"ENEMYSPAWNER: ENEMY WITH ID OF {EnemyID} DOES NOT EXIST");
            return null;
        }

        SummonedEnemy.ID = EnemyID;
        EnemiesInGame.Add(SummonedEnemy);
        EnemiesInGameAsTransforms.Add(SummonedEnemy.transform);
        return SummonedEnemy;
    }

    public void RemoveEnemy(Enemy EnemyToRemove)
    {
        EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
        EnemyToRemove.gameObject.SetActive(false);
        EnemiesInGame.Remove(EnemyToRemove);
        EnemiesInGameAsTransforms.Remove(EnemyToRemove.transform);
    }
}
