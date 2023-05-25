using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    private GameLoopManager gameLoopManager;

    public enum TargetType
    {
        First,
        Last,
        Close
    }

    public void OnEnable()
    {
        gameLoopManager = GetComponentInParent<GameLoopManager>();
    }

    public Enemy GetTarget(Tower CurrentTower, TargetType TargetType)
    {
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position, CurrentTower.Range, CurrentTower.EnemiesLayer);
        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> WaypointPositions = new NativeArray<Vector3>(gameLoopManager.WaypointPositions, Allocator.TempJob);
        NativeArray<float> WaypointDistances = new NativeArray<float>(gameLoopManager.WaypointDistances, Allocator.TempJob);
        NativeArray<int> EnemyToIndex = new NativeArray<int>(new int[] { -1 }, Allocator.TempJob);
        int EnemyIndexToReturn = -1;

        for (int i = 0; i < EnemiesToCalculate.Length; i++)
        {
            Enemy currentEnemy = EnemiesInRange[i].GetComponentInParent<Enemy>();
            int EnemyIndexInList = gameLoopManager.EnemySpawner.EnemiesInGame.FindIndex(x => x == currentEnemy);
            EnemiesToCalculate[i] = new EnemyData(currentEnemy.transform.position, currentEnemy.WaypointIndex, EnemyIndexInList, currentEnemy.Health);
        }

        SearchForEnemy SearchForEnemyJob = new SearchForEnemy
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _WaypointPositions = WaypointPositions,
            _WaypointDistances = WaypointDistances,
            _EnemyToIndex = EnemyToIndex,
            _TowerPosition = CurrentTower.transform.position,
            _TargetType = TargetType
        };

        switch (TargetType)
        {
            case TargetType.First:
                SearchForEnemyJob._CompareValue = Mathf.Infinity;
                break;
            case TargetType.Last:
                SearchForEnemyJob._CompareValue = Mathf.NegativeInfinity;
                break;
            case TargetType.Close:
                goto case TargetType.First;
        }

        JobHandle dependency = new JobHandle();
        JobHandle SearchForEnemyJobHandle = SearchForEnemyJob.Schedule(EnemiesToCalculate.Length, dependency);

        SearchForEnemyJobHandle.Complete();

        EnemyIndexToReturn = EnemiesToCalculate[EnemyToIndex[0]].EnemyIndex;

        EnemiesToCalculate.Dispose();
        WaypointPositions.Dispose();
        WaypointDistances.Dispose();
        EnemyToIndex.Dispose();

        if (EnemyIndexToReturn == -1)
        {
            return null;
        }

        return gameLoopManager.EnemySpawner.EnemiesInGame[EnemyIndexToReturn];
    }


    struct EnemyData
    {
        public EnemyData(Vector3 position, int waypointIndex, int enemyIndex, float health)
        {
            EnemyPosition = position;
            WaypointIndex = waypointIndex;
            EnemyIndex = enemyIndex;
            Health = health;
        }

        public Vector3 EnemyPosition;
        public int WaypointIndex;
        public int EnemyIndex;
        public float Health;
    }

    struct SearchForEnemy : IJobFor
    {
        public NativeArray<EnemyData> _EnemiesToCalculate;
        public NativeArray<Vector3> _WaypointPositions;
        public NativeArray<float> _WaypointDistances;
        public NativeArray<int> _EnemyToIndex;
        public Vector3 _TowerPosition;
        public float _CompareValue;
        public TargetType _TargetType;

        public void Execute(int index)
        {
            float distance = 0;
            switch (_TargetType)
            {
                case TargetType.First:
                    distance = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (distance < _CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        _CompareValue = distance;
                    }
                    break;
                case TargetType.Last:
                    distance = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (distance > _CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        _CompareValue = distance;
                    }
                    break;
                case TargetType.Close:
                    distance = Vector3.Distance(_TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (distance > _CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        _CompareValue = distance;
                    }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData enemyToEvaluate)
        {
            float FinalDistance = Vector3.Distance(enemyToEvaluate.EnemyPosition, _WaypointPositions[enemyToEvaluate.WaypointIndex]);

            for (int i = enemyToEvaluate.WaypointIndex; i < _WaypointDistances.Length; i++)
            {
                FinalDistance += _WaypointDistances[i];
            }

            return FinalDistance;
        }
    }
}
