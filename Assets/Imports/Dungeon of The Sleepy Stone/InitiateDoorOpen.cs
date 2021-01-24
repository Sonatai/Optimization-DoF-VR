using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitiateDoorOpen : MonoBehaviour
{
    public Animator anim;
    public Collider floor;
    [SerializeField]
    private NavMeshObstacle navCollider;


    void Start()
    {
        anim.SetBool("Open", true);
        //floor.enabled = true;
        if (navCollider != null)
            navCollider.enabled = false;
    }

    
}
