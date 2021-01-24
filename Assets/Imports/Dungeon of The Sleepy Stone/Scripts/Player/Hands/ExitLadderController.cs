using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;


public class ExitLadderController : Triggerable
{
    private bool _enteredNextLevel = false;
    public override void Trigger(HandController hand)
    {
        if (!_enteredNextLevel)
        {
            _enteredNextLevel = true;
            (FindObjectOfType(typeof(FadingScript)) as FadingScript).NextLevel();
        }
    }
}
