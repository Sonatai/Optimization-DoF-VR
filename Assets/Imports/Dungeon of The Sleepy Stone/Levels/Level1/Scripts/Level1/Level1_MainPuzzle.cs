using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1_MainPuzzle : MonoBehaviour
{
    public bool crossbowFound = false;
    public bool quiverFound = false;
    [SerializeField] private NextScene _nextScene;

    public void CheckOpenExit()
    {
        if (crossbowFound && quiverFound)
        {
            _nextScene.TriggerNextScene();
        }
    }
}