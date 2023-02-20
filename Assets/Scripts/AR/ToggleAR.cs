using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Management;

public class ToggleAR : MonoBehaviour
{
    [Header("State Objects")]
    [SerializeField] Camera mainCamera;
    [SerializeField] ARController ARController;

    [Header("State Variables")]
    [SerializeField] bool ARIsEnabled;

    [Header("Toggle Button")]
    [SerializeField] Image toggleButtonFill;
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;

    private XRManagerSettings settings;
    private ARSession session;

    private void Start()
    {
        settings = XRGeneralSettings.Instance.Manager;
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

            // Enable and initialize our AR stuff
            ARController.gameObject.SetActive(true);
            session.enabled = true;
            settings.InitializeLoaderSync();
        }
        else
        {
            // Enable our in game camra
            mainCamera.gameObject.SetActive(true);

            // Disable out AR stuff
            settings.DeinitializeLoader();
            session.Reset();
            session.enabled = false;
            ARController.gameObject.SetActive(false);
        }
    }
}
