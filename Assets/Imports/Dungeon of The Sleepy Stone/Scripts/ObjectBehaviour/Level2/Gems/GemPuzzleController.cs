using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GemPuzzleController : MonoBehaviour
{
    [FormerlySerializedAs("gemWall1")] public GemBaseController gemBase1;
    [FormerlySerializedAs("gemWall2")] public GemBaseController gemBase2;
    [FormerlySerializedAs("gemWall3")] public GemBaseController gemBase3;

    public GameObject hiddenWall1;
    public GameObject hiddenWall2;


    private bool isSolvedGem = false;
    

    /**
     * Note: This is level 2 specific!...
     */

    public void CheckIfSolved()
    {
        if (!isSolvedGem)
        {
            if (gemBase1.isSolved && gemBase2.isSolved && gemBase3.isSolved)
            {
                hiddenWall1.SetActive(false);
                hiddenWall2.SetActive(false);
                isSolvedGem = true;
            }
        }
    }
    
    
    
}
