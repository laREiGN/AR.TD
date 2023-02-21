using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARToggle : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject levelContainer;

    [Header("State Objects")]
    [SerializeField] Camera mainCamera;
    [SerializeField] ARController ARController;

    [Header("Toggle Button")]
    [SerializeField] Image toggleButtonFill;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;

    [HideInInspector] public bool ARIsEnabled;

    private ARSession session;

    private void Start()
    {
        session = ARController.GetComponentInChildren<ARSession>();
    }
    private void Update()
    {
        if (ARIsEnabled)
        {
            if (toggleButtonFill.color != enabledColor)
            {
                toggleButtonFill.color = enabledColor;
            }
        }
        else
        {
            if (toggleButtonFill.color != disabledColor)
            {
                toggleButtonFill.color = disabledColor;
            }
        }
    }

    public void ToggleARState(bool isEnabled)
    {
        ARIsEnabled = isEnabled;

        if (ARIsEnabled)
        {
            // Disable our in game camera
            mainCamera.gameObject.SetActive(false);

            // Disable our level till we wait for first click
            levelContainer.SetActive(false);

            // Enable and initialize our AR stuff
            ARController.gameObject.SetActive(true);
            session.enabled = true;
            session.Reset();
        }
        else
        {
            // Enable our level and zero our position
            levelContainer.SetActive(true);
            levelContainer.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            // Enable our in game camra
            mainCamera.gameObject.SetActive(true);

            // Disable out AR stuff
            session.enabled = false;
            ARController.gameObject.SetActive(false);
        }
    }
}
