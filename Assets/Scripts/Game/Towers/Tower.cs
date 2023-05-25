using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public LayerMask EnemiesLayer;

    public Enemy currentTarget;
    public GameObject towerBody;
    public int ID;
    public bool IsPlacedDown;

    public int Level;
    public float Damage;
    public float Firerate;
    public float Range;
    private float Delay;

    public void Init(int id)
    {
        ID = id;
        IsPlacedDown = false;
        towerBody.SetActive(false);
        Level = 0;
        Delay = 1 / Firerate;
    }

    public void PlaceTower()
    {
        IsPlacedDown = true;
        towerBody.SetActive(true);
    }

    public void Tick()
    {
        if (IsPlacedDown && currentTarget != null)
        {
            towerBody.transform.rotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
        }
    }
}
