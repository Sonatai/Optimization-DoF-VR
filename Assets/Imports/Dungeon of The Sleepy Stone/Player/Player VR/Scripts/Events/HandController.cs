using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandController : MonoBehaviour
{
    private List<Interactable> _collidingInteractables = new List<Interactable>();
    private Interactable _interactingObjectInteractable;
    private Grabable _objectInHandGrabable;

    public bool inputsEnabled = true;


    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean triggerButtonTrigger; //Grab Pinch is the trigger, select from inspecter
    public SteamVR_Action_Boolean gripButtonTrigger; //Grab Pinch is the trigger, select from inspecter
    public SteamVR_Input_Sources inputSource; //which controller


    void OnEnable()
    {
        if (triggerButtonTrigger != null)
        {
            triggerButtonTrigger.AddOnChangeListener(OnTriggerButtonUsed, inputSource);
        }

        if (gripButtonTrigger != null)
        {
            gripButtonTrigger.AddOnChangeListener(OnGripButtonUsed, inputSource);
        }
    }

    private void OnDisable()
    {
        if (triggerButtonTrigger != null)
        {
            triggerButtonTrigger.RemoveOnChangeListener(OnTriggerButtonUsed, inputSource);
        }

        if (gripButtonTrigger != null)
        {
            gripButtonTrigger.RemoveOnChangeListener(OnGripButtonUsed, inputSource);
        }
    }

    private void OnTriggerButtonUsed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        TriggerButtonAtInteracting(newState ? ButtonEventKind.TriggerButtonDown : ButtonEventKind.TriggerButtonUp);
    }

    private void OnGripButtonUsed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        TriggerButtonAtInteracting(newState ? ButtonEventKind.GrabButtonDown : ButtonEventKind.GrabButtonUp);
    }

    void OnJointBreak(float breakForce)
    {
        _objectInHandGrabable.ReleaseFromHand();

        if (_objectInHandGrabable)
        {
            ReleaseObject();
        }
    }

    private void TriggerButtonAtInteracting(ButtonEventKind buttonEvent)
    {
        if (inputsEnabled)
        {
            if (_objectInHandGrabable && _objectInHandGrabable.releaseButtonAction == buttonEvent)
            {
                _objectInHandGrabable.ButtonUsed(this, buttonEvent);
            }
            else if (_interactingObjectInteractable)
            {
                _interactingObjectInteractable.ButtonUsed(this, buttonEvent);
            }
        }
    }

    public void EnterObject(GameObject collidingObject)
    {
        var collidingObjectInteractableScript = collidingObject.GetComponent<Interactable>();

        if (collidingObjectInteractableScript && !_collidingInteractables.Contains(collidingObjectInteractableScript))
        {
            _collidingInteractables.Add(collidingObjectInteractableScript);
            _interactingObjectInteractable = collidingObject.GetComponent<Interactable>();
        }
    }

    public void ExitObject(GameObject exitingObject)
    {
        var exitingObjectInteractableScript = exitingObject.GetComponent<Interactable>();

        if (exitingObjectInteractableScript)
        {
            _collidingInteractables.Remove(exitingObjectInteractableScript);

            if (_collidingInteractables.Count > 0)
            {
                _interactingObjectInteractable = _collidingInteractables[_collidingInteractables.Count - 1];
            }
            else if (_objectInHandGrabable)
            {
                _interactingObjectInteractable = _objectInHandGrabable;
            }
            else
            {
                _interactingObjectInteractable = null;
            }
        }
    }

    public void AttachObject(GameObject collidingObject)
    {
        if (_objectInHandGrabable)
        {
            _objectInHandGrabable.ReleaseFromHand();
            //releases object if grabable didnt release in ReleaseFromHand
            ReleaseObject();
        }

        _objectInHandGrabable = collidingObject.GetComponent<Grabable>();

        CreateFixedJoint(collidingObject);
    }

    public void ReleaseObject()
    {
        if (_objectInHandGrabable)
        {
            DestroyFixedJoint();

            //Some objects get immediately destroyed and would never be exited
            ExitObject(_objectInHandGrabable.gameObject);

            _objectInHandGrabable = null;
        }
    }

    private void CreateFixedJoint(GameObject collidingObject)
    {
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.breakForce = float.PositiveInfinity;
        joint.breakTorque = float.PositiveInfinity;
        joint.connectedBody = collidingObject.GetComponent<Rigidbody>();
    }

    private void DestroyFixedJoint()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            var inHandRigidbody = _objectInHandGrabable.gameObject.GetComponent<Rigidbody>();
            inHandRigidbody.velocity = controllerPose.GetVelocity() * 2.5f;
            inHandRigidbody.angularVelocity = controllerPose.GetAngularVelocity();
        }
    }
}