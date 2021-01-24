using System;
using UnityEngine;
public class LadderController : Triggerable
{
    public Vector3 groundPosition;
    public Vector3 topPosition;
    private bool _isOnTopLevel = false;
    private WallCollision wallCollision;

    public void Start()
    {
        wallCollision = (FindObjectOfType(typeof(WallCollision)) as WallCollision);
    }

    public override void Trigger(HandController hand)
    {
        GameObject player = GameObject.Find("PlayerVR"); //TODO: Test if this is needed.
        
        if (player.transform.position.y > gameObject.transform.position.y)
        {
            player.transform.position = groundPosition;
        }
        else
        {
            player.transform.position = topPosition;
        }
        
        if (wallCollision)
        {
            wallCollision.UpdatePreviousPosition();
        }
    }
}