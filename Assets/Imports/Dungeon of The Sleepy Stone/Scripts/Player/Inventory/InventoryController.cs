using System;
using UnityEngine;
using Valve.VR;

public class InventoryController : MonoBehaviour
{
    [Header("Controller Config")]
    [SerializeField]
    private SteamVR_Input_Sources handType;

    [Header("Actions")]
    [SerializeField]
    private SteamVR_Action_Boolean touch = null;
    [SerializeField]
    private SteamVR_Action_Boolean press = null;
    [SerializeField]
    private SteamVR_Action_Vector2 touchPosition = null;

    [Header("Scene Objects")]
    private RadialMenu radialMenu;

    public RadialMenu SetRadialMenu
    {
        set=>  radialMenu = value;
    }
    public void AddListeners()
    {
        touch.AddOnChangeListener(Touch, handType);
        press.AddOnStateDownListener(Press, handType);
        press.AddOnStateUpListener(PressRelease, handType);
        touchPosition.AddOnAxisListener(Position, handType);
    }
    public void RemoveListeners()
    {
        touch.RemoveOnChangeListener(Touch,handType);
        press.RemoveOnStateDownListener(Press,handType);
        press.RemoveOnStateUpListener(PressRelease, handType);
        touchPosition.RemoveOnAxisListener(Position,handType);
    }

    private void Position(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        if (radialMenu)
        {
            radialMenu.SetTouchPosition(axis);
        }
    }

    private void Touch(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (radialMenu)  //Brauchen wir, damit das Invi funktioniert
        {
            radialMenu.Show(newState);
        }
    }

    private void Press(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (radialMenu) //Brauchen wir, damit das Invi funktioniert
        {
            radialMenu.ActivateHighlightedSection();
        }
    }

    private void PressRelease(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (radialMenu)  //Brauchen wir, damit das Invi funktioniert
        {
            radialMenu.Show(true);
        }
    }

    private void OnDestroy()
    {
        radialMenu = null;
    }
}
