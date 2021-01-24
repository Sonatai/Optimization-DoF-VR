using UnityEngine;

public class FireQuiv : Triggerable
{
    public override void Trigger(HandController hand)
    {
        GameObject.Find("Controller (left)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(0);
        GameObject.Find("Controller (right)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(0);
        
        hand.ExitObject(gameObject);
        
        Destroy(gameObject);
    }
}