using UnityEngine;
using Valve.VR;

public class ControllerActionsTest : MonoBehaviour
{
    public SteamVR_Input_Sources handType; 
    public SteamVR_Action_Boolean teleportAction; 
    public SteamVR_Action_Boolean grabAction; 

    private void Update()
    {
        if (GetTeleportDown())
        {
            print("Teleport " + handType);
        }

        if (GetGrab())
        {
            print("Grab " + handType);
        }
    }

    public bool GetTeleportDown()
    {
        return teleportAction.GetLastStateDown(handType);
    }

    public bool GetGrab()
    {
        return grabAction.GetLastState(handType);
    }
}
