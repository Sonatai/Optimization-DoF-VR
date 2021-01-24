using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GemBaseController : MonoBehaviour
{
    [FormerlySerializedAs("puzzle")] public GemPuzzleController puzzleController;
    //public GameObject wall;
    [FormerlySerializedAs("objectTag")] public string gemTag;
    public GameObject dummyGem;
    public bool isSolved = false;

    private void Start()
    {
        puzzleController = FindObjectOfType(typeof(GemPuzzleController)) as GemPuzzleController;
    }
    public void OnEnter(Collider other)
    {
        
    
        if (other.gameObject.CompareTag(gemTag))
        {
            var gemScript = other.gameObject.GetComponent<Gem>();
            
            gemScript.OnGemBaseEnter(gameObject);
        }
    }

    public void OnExit(Collider other)
    {
        if (other.gameObject.CompareTag(gemTag))
        {
            var gemScript = other.gameObject.GetComponent<Gem>();
            
            gemScript.OnGemBaseExit(gameObject);
        }
    } 

    public void OnTriggerEnter(Collider other)
    {
        OnEnter(other);
    }

    public void OnTriggerStay(Collider other)
    {
        OnEnter(other);
    }

    public void OnTriggerExit(Collider other)
    {
        OnExit(other);
    }

    public void AttachGem()
    {
        isSolved = true;
        puzzleController.CheckIfSolved();
        dummyGem.SetActive(true);
    }

}
