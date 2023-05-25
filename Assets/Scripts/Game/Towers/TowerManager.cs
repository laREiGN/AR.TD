using System.Collections.Generic;
using UnityEngine;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class TowerManager : MonoBehaviour
{
    private Tower[] Towers;

    [Header("AR Components")]
    public Camera ARCamera;

    [Header("Non-AR Components")]
    public Camera Camera;

    [Header("Other stuff")]
    public Transform TowerContainer;

    private GameLoopManager GameLoopManager;
    private bool IsInitialized;

    private void OnEnable()
    {
        GameLoopManager = GetComponentInParent<GameLoopManager>();

        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += ListenForTowerSelect;
    }

    private void OnDisable()
    {
        EnhancedTouch.Touch.onFingerDown -= ListenForTowerSelect;
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.TouchSimulation.Disable();
    }

    public void Init()
    {
        if (!IsInitialized)
        {
            Towers = new Tower[TowerContainer.childCount];
            for (int i = 0; i < Towers.Length; i++) {
                Tower tower = TowerContainer.GetChild(i).GetComponent<Tower>();
                tower.Init(i);
                Towers[i] = tower;
            }

            IsInitialized = true;
        }
    }

    public void ListenForTowerSelect(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return; // Only listen to our first touch

        Ray ray;
        if (ARToggle.ARIsEnabled)
        {
            ray = ARCamera.ScreenPointToRay(finger.currentTouch.screenPosition);
        } 
       
        else
        {
            ray = Camera.ScreenPointToRay(finger.currentTouch.screenPosition);
        }
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Tower"))
            {
                Tower tower = hit.collider.GetComponent<Tower>();
                if (!tower.IsPlacedDown)
                {
                    tower.PlaceTower();
                    GameLoopManager.activeTowers.Add(tower);
                }
            }
        }
    }
}
