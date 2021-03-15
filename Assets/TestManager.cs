using System;
using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.FoveatedRendering;
using UnityEngine;
using UnityEngine.Serialization;

public class TestManager : MonoBehaviour
{
    //TODO: Fov Script, DoF Script, optimiertes DoF Script
    [SerializeField] private DofAdaptiveRecursiveFiltering dofScript;
    [SerializeField] private ViveFoveatedRendering fovScript;
    [SerializeField] private TestMode testMode;
    private TestMode _oldTestMode;

    private void Update()
    {
        if (_oldTestMode == testMode)
        {
            return;
        }

        if (testMode == TestMode.Baseline)
        {
            //TODO: Nothing on
            _oldTestMode = testMode;
        }

        if (testMode == TestMode.Fall1)
        {
            //TODO: DofScript
            _oldTestMode = testMode;
        }

        if (testMode == TestMode.Fall2)
        {
            //TODO: Fov Script
            _oldTestMode = testMode;
        }

        if (testMode == TestMode.Optimierung)
        {
            //TODO: Fov Script + Optimiertes DofScript
            _oldTestMode = testMode;
        }
        
    }
}

enum TestMode
{
    Baseline,
    Fall1,
    Fall2,
    Optimierung
    
}
