 using System;
 using UnityEngine;

public class Gem : Grabable
{
    public String baseTag;
    private GameObject collidingBase;

    public void OnGemBaseEnter(GameObject gemBase)
    {
        if (gemBase.GetComponent<GemBaseController>() && attachedHand)
        {
            collidingBase = gemBase.gameObject;
            
        }
    }

    public void OnGemBaseExit(GameObject gemBase)
    {
        if (gemBase.GetComponent<GemBaseController>() && attachedHand)
        {
            collidingBase = null;
        }
    }

    public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
    {
        if (buttonEventKind == ButtonEventKind.TriggerButtonDown)
        {
            AttachToHand(hand);
            
        }
        else if (buttonEventKind == ButtonEventKind.TriggerButtonUp) //... wird beim colliding nicht ausgeführt?
        {
            ReleaseFromHand();
            if (collidingBase)
            {

                GemBaseController gemBaseBaseController = collidingBase.GetComponent<GemBaseController>();

                gemBaseBaseController.AttachGem();
                //gem gets replaced by dummy gem in base and is not needed anymore
                Destroy(gameObject);
            }
        }
    }
}