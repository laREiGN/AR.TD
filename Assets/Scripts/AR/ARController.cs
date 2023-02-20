using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class ARController : MonoBehaviour
{
    public Transform levelObject;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private Camera ARCamera;
    private List<ARRaycastHit> hits = new();

    //TODO: Make script LevelPlacer or something like that
    //TODO: Make this only listen for one placement click (maybe a button somewhere to reset, or a double tap/hold to place again?)

    private void Awake()
    {
        raycastManager = GetComponentInChildren<ARRaycastManager>();
        planeManager = GetComponentInChildren<ARPlaneManager>();
        ARCamera = GetComponentInChildren<Camera>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += PlaceLevel;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= PlaceLevel;
    }

    private void PlaceLevel(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return; // Only listen to our first touch

        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            if (hit != null)
            {
                Pose pose = hit.pose;
                levelObject.SetPositionAndRotation(pose.position, pose.rotation);
                if (planeManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 position = levelObject.transform.position;
                    Vector3 cameraPosition = ARCamera.transform.position;
                    Vector3 direction = cameraPosition - position;
                    Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
                    Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, levelObject.transform.up.normalized); // (0, 1, 0)
                    Quaternion targetRotation = Quaternion.Euler(scaledEuler);
                    levelObject.transform.rotation = levelObject.transform.rotation * targetRotation;
                }
            }
        }
    }
}
