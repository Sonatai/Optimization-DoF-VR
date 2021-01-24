using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadingScript : MonoBehaviour
{
    
    private bool _enteredNextLevel = false;
    public Animator animator;
    private int LevelToLoad;

    private void Start()
    {
        LevelToLoad = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("Current Scene: " + LevelToLoad);
        Debug.Log("Scene Count: " + SceneManager.sceneCountInBuildSettings);
    }

    //initiates fade out and loads the next scene/level right after
    public void NextLevel()
    {
        if (!_enteredNextLevel)
        {
            _enteredNextLevel = true;
            LevelToLoad += 1;
            if (SceneManager.sceneCountInBuildSettings > LevelToLoad)
            {
                Debug.Log("New Level");
                animator.SetTrigger("FadeOutTrigger");
            }
            else
            {
                Debug.Log("End");
                animator.SetTrigger("EscapeTrigger");
            }
        }
    }    

    //initiates fade out and restarts the current scene
    public void Death()
    {
        LevelToLoad = SceneManager.GetActiveScene().buildIndex;
        animator.SetTrigger("DeathTrigger");
    }

    private void LoadNewLevel()
    {
        
        if (SceneManager.sceneCountInBuildSettings > LevelToLoad)
        {
            SceneManager.LoadScene(LevelToLoad);
        }
        else
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }
    }
}
