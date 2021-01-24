using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class NextScene : MonoBehaviour
{
    public Collider triggerNextScene;
    private bool _enteredNextLevel = false;
    


    public void TriggerNextScene()
    {
        triggerNextScene.enabled = true;
        
    }
    private void OnTriggerStay(Collider other) {
        if (!_enteredNextLevel || other.CompareTag ("Player") || other.CompareTag("GameController"))
        {
            _enteredNextLevel = true;
            (FindObjectOfType(typeof(FadingScript)) as FadingScript).NextLevel();
        }
    }
}
