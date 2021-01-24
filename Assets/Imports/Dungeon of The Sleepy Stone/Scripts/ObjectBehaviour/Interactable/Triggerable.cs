using UnityEngine;

public abstract class Triggerable : Interactable
{
    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown)
        {
            Trigger(hand);
        }
    }

    public abstract void Trigger(HandController hand);
}