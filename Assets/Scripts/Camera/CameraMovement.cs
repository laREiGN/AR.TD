using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float cameraDistance;
    [SerializeField] float cameraHeight;

    private Camera mainCamera;
    private Vector3 previousPosition;

    //TODO: Swap this input shit to new Input System + EnhancedTouch

    private void OnEnable()
    {
        mainCamera = GetComponent<Camera>();

        mainCamera.transform.position = cameraTarget.position;
        mainCamera.transform.Translate(new Vector3(0, -cameraHeight, -cameraDistance));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = previousPosition - mainCamera.ScreenToViewportPoint(Input.mousePosition);

            mainCamera.transform.position = cameraTarget.position;

            mainCamera.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
            mainCamera.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
            mainCamera.transform.Translate(new Vector3(0, -cameraHeight, -cameraDistance));

            previousPosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
