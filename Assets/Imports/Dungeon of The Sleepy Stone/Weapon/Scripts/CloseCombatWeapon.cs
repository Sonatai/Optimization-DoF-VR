using UnityEngine;

public class CloseCombatWeapon : Grabable
{
    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown && !attachedHand)
        {
            transform.position = hand.gameObject.transform.position;
            transform.rotation = hand.gameObject.transform.rotation;
            
            AttachToHand(hand);
        }
        else if (buttonEventKind == ButtonEventKind.GrabButtonDown && attachedHand)
        {
            ReleaseFromHand();
        }
    }

    protected override void OnOtherEnter(Collider other)
    {
        other.gameObject.GetComponent<EnemyBehavior>().Hit(10, gameObject.transform.position);
    }
}