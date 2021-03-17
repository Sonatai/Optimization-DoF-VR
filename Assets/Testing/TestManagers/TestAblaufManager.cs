using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAblaufManager : MonoBehaviour
{
    [Header("Test Ablauf")] 
    [SerializeField] private Positions userPosition;
    [SerializeField] private Phases testPhase;
    private TestPosition _currentTestPosition;
    private Positions _oldPosition;
    
    [Header("General")] 
    [SerializeField] private GameObject vrUser;
    [SerializeField] private DofAdaptiveRecursiveFiltering dofScript;
    [SerializeField] private Transform vrUserHeadPosition;
    
    [Header("Test Position")] 
    [SerializeField] private TestPosition testPosition1;
    [SerializeField] private TestPosition testPosition2;
    [SerializeField] private TestPosition testPosition3;

    private void FixedUpdate()
    {
        if (userPosition != _oldPosition)
        {
            switch (userPosition)
            {
                case Positions.position1:
                    _currentTestPosition = testPosition1;
                    break;
                case Positions.position2:
                    _currentTestPosition = testPosition2;
                    break;
                case Positions.position3:
                    _currentTestPosition = testPosition3;
                    break;
                default:
                    Debug.Log("Something is terrible wrong 🤢🤢🤢🤢");
                    break;
            }

            _oldPosition = userPosition;
            vrUser.transform.position = _currentTestPosition.UserPosition.position;
        }
        
        switch (testPhase)
        {
            case Phases.phase1:
                Focus(_currentTestPosition.FocusPoint1);
                break;
            case Phases.phase2:
                Focus(_currentTestPosition.FocusPoint2);
                break;
            default:
                Debug.Log("Something is terrible wrong 🤢🤢🤢🤢");
                break;
        }

    }
    
    private void Focus(Transform currentFocusPoint)
    {
        Vector3 focalVec = currentFocusPoint.position - vrUserHeadPosition.position;
        var focalLength = (float) Math.Sqrt(Math.Pow(focalVec.x, 2) + Math.Pow(focalVec.y, 2) + Math.Pow(focalVec.z, 2));
        dofScript.FocalLength = focalLength;
    }
}

enum Phases
{
    phase1,
    phase2
}

enum Positions
{
    position1,
    position2,
    position3
}