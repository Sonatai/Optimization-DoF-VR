using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.FoveatedRendering;
using UnityEngine;
using UnityEngine.Serialization;

public class TestModeManager : MonoBehaviour
{
    [Header("Test Mode")]
    [SerializeField] private TestMode testMode;
    private TestMode _oldTestMode;
    
    [Header("General")] 
    [SerializeField] private DofAdaptiveRecursiveFiltering dofScript;
    [SerializeField] private ViveFoveatedRendering fovScript; 
    [SerializeField] private TestAblaufManager testAblaufManager;
    
    private void FixedUpdate()
    {
        if (_oldTestMode == testMode)
        {
            return;
        }

        switch (testMode)
        {
            case TestMode.Baseline:
                dofScript.enabled = false;
                fovScript.enabled = false;
                testAblaufManager.enabled = false;
                break;
            case TestMode.Fall1:
                //TODO: Dof Script => make it config to enabeld/disbaled optimize shader :)
                fovScript.enabled = false;
                testAblaufManager.enabled = true;
                break;
            case TestMode.Fall2:
                //TODO: Fov Script +  DofScript
                fovScript.enabled = true;
                testAblaufManager.enabled = true;
                break;
            case TestMode.Optimierung:
                //TODO: Fov Script + optimized Dof Script
                fovScript.enabled = true;
                testAblaufManager.enabled = true;
                break;
            default:
                Debug.Log("Something is terrible wrong 🤢🤢🤢🤢");
                break;
        } 
        _oldTestMode = testMode;
    }
}

enum TestMode
{
    Baseline,
    Fall1,
    Fall2,
    Optimierung
    
}