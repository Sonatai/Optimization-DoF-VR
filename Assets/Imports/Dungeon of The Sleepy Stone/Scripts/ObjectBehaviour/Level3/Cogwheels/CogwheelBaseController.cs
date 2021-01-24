using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Valve.VR.InteractionSystem;
 
[System.Serializable]
public class NeighbourCogwheelBase
{
    public GameObject cogwheelFoundation;
    public float distance;

}

[System.Serializable]
public class CogwheelBaseController : MonoBehaviour
{
    public enum SpinDirection
    {
        Left,
        Right
    }
    [Serializable]
    public class LadderPairs
    {
        public GameObject closedLadder;
        public GameObject openedLadder;
    }
    
    public NeighbourCogwheelBase[] neighbourCogwheelBases;
    public GameObject cogwheel;
    [HideInInspector]
    public float cogwheelRadius = 0;
    private bool _spinning = false;
    private SpinDirection _spinDirection;
    public bool isActionCogwheel;
    public LadderPairs[] ladders;

    private void Awake()
    {
        if (cogwheel)
        {
            AttachCogwheel(cogwheel);
        }
    }

    public void OnEnter(Collider other)
    {
        
        //Debug.Log("Base Colliding with " + other.gameObject.tag);
        if (other.gameObject.CompareTag("Cogwheel"))
        {
            var cogwheelController = other.gameObject.GetComponent<CogwheelController>();
            
            cogwheelController.OnCogwheelBaseEnter(gameObject);
        }
    }

    public void OnExit(Collider other)
    {
        if (other.gameObject.CompareTag("Cogwheel"))
        {
            var cogwheelController = other.gameObject.GetComponent<CogwheelController>();
            
            cogwheelController.OnCogwheelBaseExit(gameObject);
        }
    } 

    public void OnTriggerEnter(Collider other)
    {
        OnEnter(other);
    }

    public void OnTriggerStay(Collider other)
    {
        OnEnter(other);
    }

    public void OnTriggerExit(Collider other)
    {
        OnExit(other);
    }

    public bool CanAttachCogwheel(GameObject cogwheel)
    {
        if (cogwheel.CompareTag("Cogwheel"))
        {
            CogwheelController cogwheelController = cogwheel.GetComponent<CogwheelController>();

            if (cogwheelController != null)
            {

                if (IsCogwheelSmallEnough(cogwheelController))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsCogwheelSmallEnough(CogwheelController cogwheelController)
    {
        foreach (var neighbourCogwheelFoundation in neighbourCogwheelBases)
        {
            if (
                neighbourCogwheelFoundation.distance < cogwheelController.radius +
                neighbourCogwheelFoundation.cogwheelFoundation
                    .GetComponent<CogwheelBaseController>().cogwheelRadius
            )
            {
                return false;
            }
        }

        return true;
    }

    public void AttachCogwheel(GameObject cogwheel)
    {
        if (CanAttachCogwheel(cogwheel))
        {
            CogwheelController cogwheelController = cogwheel.GetComponent<CogwheelController>();
        
            if(this.cogwheel)
            {
                ReleaseCogwheel();
            }
        
            this.cogwheel = cogwheel;
            cogwheelRadius = cogwheelController.radius;

            this.cogwheel.GetComponent<Rigidbody>().isKinematic = true;
            cogwheelController.baseAttachedTo = gameObject;

            this.cogwheel.transform.position = transform.position;
            this.cogwheel.transform.rotation = transform.rotation;
        }
    }

    public void ReleaseCogwheel()
    {
        if (CanReleaseCogwheel())
        {
            cogwheel.GetComponent<Rigidbody>().isKinematic = false;
            cogwheel.GetComponent<CogwheelController>().baseAttachedTo = null;
        
            cogwheel = null;
            cogwheelRadius = 0;
        }
    }

    public bool CanReleaseCogwheel()
    {
        return !_spinning;
    }
    
    public void Spin(SpinDirection spinDirection)
    {
        if (cogwheel && !_spinning)
        {
            _spinning = true;
            _spinDirection = spinDirection;

            SpinNeighbours();

            if (isActionCogwheel)
            {
                foreach (var ladderPair in ladders)
                {
                    ladderPair.closedLadder.SetActive(false);
                    ladderPair.openedLadder.SetActive(true);
                }
            }
        }
        else if (_spinDirection != spinDirection)
        {
            StopSpin();
        }
    }

    private void SpinNeighbours()
    {
        foreach (NeighbourCogwheelBase neighbourCogwheelFoundation in neighbourCogwheelBases)
        {
            if (_spinning)
            {
                neighbourCogwheelFoundation.cogwheelFoundation.GetComponent<CogwheelBaseController>()
                    .Spin(
                        neighbourCogwheelFoundation.distance - cogwheelRadius,
                        _spinDirection == SpinDirection.Right ? SpinDirection.Left : SpinDirection.Right
                    );
            }
        }
    }

    public void Spin(float neededCogwheelRadius, SpinDirection spinDirection)
    {
        if (cogwheel && Math.Abs(neededCogwheelRadius - cogwheelRadius) < 0.1)
        {
            Spin(spinDirection);
        }
    }

    public void StopSpin()
    {
        if (cogwheel && _spinning)
        {
            _spinning = false;

            StopSpinningNeighbours();
        }
    }

    private void StopSpinningNeighbours()
    {
        foreach (var neighbourCogwheelFoundation in neighbourCogwheelBases)
        {
            neighbourCogwheelFoundation.cogwheelFoundation.GetComponent<CogwheelBaseController>()
                .StopSpin(neighbourCogwheelFoundation.distance - cogwheelRadius);
        }
    }

    public void StopSpin(float neededCogwheelRadius)
    {
        
        if (cogwheel && Math.Abs(neededCogwheelRadius - cogwheelRadius) < 0.1)
        {
            StopSpin();
        }
    }

    private void Update()
    {
        if (_spinning)
        {
            float speed = (_spinDirection == SpinDirection.Right ? 1 : -1) * 800 * Time.deltaTime / cogwheelRadius;
            cogwheel.transform.Rotate (Vector3.up, speed);
        }
    }
}
