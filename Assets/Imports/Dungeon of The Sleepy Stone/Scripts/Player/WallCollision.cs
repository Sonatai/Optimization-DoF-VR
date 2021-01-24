using System;
using UnityEngine;
 
public class WallCollision : MonoBehaviour
{
    public LayerMask searchMask = -1; //make sure we aren't in this layer 
    private Vector3 _lastPosition;
    public bool inRoom = true;
    private bool _fadedOut = false;
    public Animator animator;
    public HandController leftHandController;
    public HandController rightHandController;
    public TeleportationPointer leftTeleportationPointer;
    public TeleportationPointer rightTeleportationPointer;

    //initialize values 
    void Awake()
    {
        UpdatePreviousPosition();
    }

    public void UpdatePreviousPosition()
    {
        _lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        RaycastHit rayHit;
        //check for obstructions we might have missed
        if (Physics.Linecast(_lastPosition, transform.position, out rayHit, layerMask: searchMask))
        {
            if (inRoom)
            {
                fadeOutIfInImmovable(rayHit);
                setInRoomStatus(false);
            }
        }
        else
        {
            UpdatePreviousPosition();
            
            if (!inRoom)
            {
                fadeInIfFadedOut();
                setInRoomStatus(true);
            }
        }
    }

    private void fadeInIfFadedOut()
    {
        if (_fadedOut)
        {
            animator.SetTrigger("InstantFadeInTrigger");
            _fadedOut = false;
        }
    }

    private void fadeOutIfInImmovable(RaycastHit rayHit)
    {
        if (rayHit.collider.gameObject.layer != LayerMask.NameToLayer("DisableTeleport"))
        {
            _fadedOut = true;
            animator.SetTrigger("InstantFadeOutTrigger");
        }
    }

    private void setInRoomStatus(bool newStatus)
    {
        inRoom = newStatus;
        
        //Debug.Log((newStatus ? "In Room" : "In Wall") + " Position: " + transform.position);

        leftHandController.inputsEnabled = newStatus;
        leftTeleportationPointer.enabled = newStatus;

        rightHandController.inputsEnabled = newStatus;
        rightTeleportationPointer.enabled = newStatus;
    }
}