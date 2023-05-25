using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARToggle : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameLoopManager gameLoopManager;
    [SerializeField] GameObject levelContainer;

    [Header("State Objects")]
    [SerializeField] Camera mainCamera;
    [SerializeField] ARController ARController;

    [Header("Toggle Button")]
    [SerializeField] Image toggleButtonFill;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;

    [HideInInspector] public static bool ARIsEnabled;

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

            // "Hide" our level and pause the game while we wait for first click
            Time.timeScale = 0;
            Vector3 hiddenPosition = new Vector3(0, 1000, 0);
            levelContainer.transform.SetPositionAndRotation(hiddenPosition, Quaternion.Euler(Vector3.zero));

            // Enable and initialize our AR stuff
            ARController.gameObject.SetActive(true);
            session.enabled = true;
            session.Reset();
        }
        else
        {
            // Zero our position and play the game
            levelContainer.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            gameLoopManager.Reload();
            Time.timeScale = 1;

            // Enable our in game camra
            mainCamera.gameObject.SetActive(true);

            // Disable out AR stuff
            session.enabled = false;
            ARController.gameObject.SetActive(false);
        }
    }
}
