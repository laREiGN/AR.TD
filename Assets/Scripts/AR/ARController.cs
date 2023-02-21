using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class ARController : MonoBehaviour
{
    public GameObject levelContainer;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private Camera ARCamera;
    private List<ARRaycastHit> hits = new();

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
        if (levelContainer.activeSelf) return; // Only listen if world is not active already

        if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits,
            TrackableType.PlaneWithinPolygon))
        {
            ARRaycastHit hit = hits[0];
            if (hit != null)
            {
                Pose pose = hit.pose;
                levelContainer.SetActive(true);
                levelContainer.transform.SetPositionAndRotation(pose.position, pose.rotation);
                if (planeManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 position = levelContainer.transform.position;
                    Vector3 cameraPosition = ARCamera.transform.position;
                    Vector3 direction = cameraPosition - position;
                    Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
                    Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, levelContainer.transform.up.normalized); // (0, 1, 0)
                    Quaternion targetRotation = Quaternion.Euler(scaledEuler);
                    levelContainer.transform.rotation = levelContainer.transform.rotation * targetRotation;
                }
            }
        }
    }
}
