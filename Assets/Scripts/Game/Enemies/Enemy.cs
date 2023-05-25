using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int WaypointIndex;
    public float MaxHealth;
    public float Health;
    public float Speed;
    public int ID;

    private GameLoopManager GameLoopManager;

    public void OnEnable()
    {
        GameLoopManager = FindObjectOfType<GameLoopManager>();
    }

    public void Init()
    {
        Health = MaxHealth;
        transform.position = GameLoopManager.WaypointPositions[0];
        WaypointIndex = 0;
    }
}
