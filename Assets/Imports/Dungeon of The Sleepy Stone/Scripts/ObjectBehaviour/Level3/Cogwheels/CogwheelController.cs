using System;
using UnityEngine;
 
 public class CogwheelController : Grabable
 {
     public float radius = 1;
     public GameObject baseAttachedTo;
     private GameObject collidingBase;
     
     public void OnCogwheelBaseEnter(GameObject cogheelBase)
     {
         if (cogheelBase.CompareTag("CogwheelBase") && attachedHand)
         {
             collidingBase = cogheelBase.gameObject;
         }
     }
     
     public void OnCogwheelBaseExit(GameObject cogheelBase)
     {
         if (cogheelBase.CompareTag("CogwheelBase") && attachedHand)
         {
             collidingBase = null;
         }
     }
     
     public override void ButtonUsed(HandController hand, ButtonEventKind buttonEventKind)
     {
         if (buttonEventKind == ButtonEventKind.TriggerButtonDown)
         {
             if (!baseAttachedTo)
             {
                 AttachToHand(hand);
             }
             else
             {
                 CogwheelBaseController attachedBaseCotroller = baseAttachedTo.GetComponent<CogwheelBaseController>();
                 if (attachedBaseCotroller.CanReleaseCogwheel())
                 {
                     attachedBaseCotroller.ReleaseCogwheel();
                     AttachToHand(hand);
                 }
             }
         }
         else if (buttonEventKind == ButtonEventKind.TriggerButtonUp)
         {
             ReleaseFromHand();
             
             if (collidingBase)
             {
                 CogwheelBaseController collidingBaseCotroller = collidingBase.GetComponent<CogwheelBaseController>();
                 
                 //Debug.Log("Try Attach");
                 if (collidingBaseCotroller.CanAttachCogwheel(gameObject))
                 {
                     //Debug.Log("Attach");
                     collidingBaseCotroller.AttachCogwheel(gameObject);
                 }
             }
         }
     }
 }