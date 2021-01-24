using UnityEngine;

public class SleepyStone : Grabable
{
    [SerializeField]
    private Pillar _pillar;
    
    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown  && !attachedHand)
        {
            AttachToHand(hand);
            _pillar.Collapse();
        }
        else if (buttonEventKind == ButtonEventKind.TriggerButtonUp  && attachedHand)
        {
            ReleaseFromHand();
        }
    }
}