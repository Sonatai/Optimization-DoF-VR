using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopePuzzleController : MonoBehaviour
{
    public Animation anim;
    public GameObject gate;
 

    private bool isSolved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isSolved)
        {
            if (other.CompareTag("Arrow"))
            {
                isSolved = true;
                anim.Play();
                //gate.isKinematic = false;
                Destroy(other.gameObject);
                gate.GetComponent<GateCloseAgent>().StartMoving();
      
                //... nav Mesh new calculation!
            }
        }
    }

}
