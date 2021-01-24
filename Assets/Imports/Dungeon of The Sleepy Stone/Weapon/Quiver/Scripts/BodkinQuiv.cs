using UnityEngine;

public class BodkinQuiv : Triggerable
{
    public override void Trigger(HandController hand)
    {
        GameObject.Find("Controller (left)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(3);
        GameObject.Find("Controller (right)").transform.Find("RadialMenu").GetComponent<RadialMenu>().ActivateButton(3);
                
        hand.ExitObject(gameObject);
        
        Destroy(gameObject);
    }
}