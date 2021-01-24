using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelExitGateController : MonoBehaviour
{
    public Animation anim;
    private bool isDestroyed = false;
    [SerializeField]
    private NavMeshObstacle navCollider;

    private void OnTriggerStay(Collider other)
    {
       
        //Debug.Log("Hi");
        //Debug.Log("Das ist der name:" + other.name);
        if (other.GetComponent<ExplodeBarrelController>() != null && !isDestroyed)
        {
            //Debug.Log("Das ist nicht NULL");
            if (other.GetComponent<ExplodeBarrelController>().isExploded)
            {
                //Debug.Log("Try to anim");
                // Debug.Log("Das ist der Start für die Anim ");
                anim.Play();
                isDestroyed = true;
                navCollider.enabled = false;
            }
        }

    }
}
