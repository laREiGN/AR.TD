using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARController : MonoBehaviour
{
    public GameObject levelObject;
    public ARRaycastManager raycastManager;

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            List<ARRaycastHit> touches = new();

            raycastManager.Raycast(Input.GetTouch(0).position, touches, TrackableType.Planes);

            if (touches.Count > 0)
            {
                levelObject.transform.SetPositionAndRotation(touches[0].pose.position, touches[0].pose.rotation);
            }
        }
    }
}
