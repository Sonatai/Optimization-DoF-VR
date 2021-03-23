using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Triggerable
{
    public Animator doorAnimator;
    [SerializeField] private NavMeshObstacle navCollider;

    private void Start()
    {
        //... dont delete!
    }

    public override void Trigger(HandController hand)
    {
        //Debug.Log("Door Trigger");

        if (doorAnimator.GetBool("Open") == false)
        {
            doorAnimator.SetBool("Open", true);
            navCollider.enabled = false;
        }
        else if (doorAnimator.GetBool("Open") == true)
        {
            doorAnimator.SetBool("Open", false);
            navCollider.enabled = true;
        }
    }
}