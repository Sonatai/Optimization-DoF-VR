using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Grabable : Interactable
{
    protected HandController attachedHand;
    private Rigidbody _rigibody;

    [SerializeField] private ButtonEventKind _releaseButtonAction = ButtonEventKind.GrabButtonUp;

    public ButtonEventKind releaseButtonAction
    {
        get => _releaseButtonAction;
        set => _releaseButtonAction = value;
    }

    private void Start()
    {
        _rigibody = gameObject.GetComponent<Rigidbody>();
    }

    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown && !attachedHand)
        {
            AttachToHand(hand);
        }
        else if (buttonEventKind == ButtonEventKind.TriggerButtonUp && attachedHand)
        {
            ReleaseFromHand();
        }
    }

    public virtual void AttachToHand(HandController hand)
    {
        hand.AttachObject(gameObject);
        attachedHand = hand;

        DisableAntiClipping();
    }

    public virtual void ReleaseFromHand()
    {
        if (attachedHand)
        {
            attachedHand.ReleaseObject();
            attachedHand = null;

            EnableAntiClipping();
        }
    }

    protected void EnableAntiClipping()
    {
        SwitchAntiClippingEnabledState(true);
    }

    protected void DisableAntiClipping()
    {
        SwitchAntiClippingEnabledState(false);
    }

    private void SwitchAntiClippingEnabledState(bool newState)
    {
        AntiClipping antiClippingScript = gameObject.GetComponent<AntiClipping>();
        if (antiClippingScript)
        {
            antiClippingScript.enabled = newState;
        }
    }
}