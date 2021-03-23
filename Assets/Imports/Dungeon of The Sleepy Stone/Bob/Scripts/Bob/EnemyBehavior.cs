﻿using System;
using Agent.Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;


/*
 * Defines Behaviour of Enemy
 */
public class EnemyBehavior : AbstractAI
{
    //public String ANIM_WALKING = "Walking";
    //public String ANIM_ATTACKING = "isAttack";
    //public String ANIM_DYING = "isDead";
    [FormerlySerializedAs("ANIMS_WALKING")]
    public String[] ANIMS_MOVING = {"Walking"};

    public String[] ANIMS_ATTACKING = {"isAttack"};
    public String[] ANIMS_DYING = {"isDead"};

    public float isIdleTimer = 0f;
    public int waypointCount = 0;
    public AudioClip[] footsounds;
    public AudioClip hit;
    public Vector3[] waypoints;

    //This function is called by the attack animation to deal damage to the player
    public void Hit()
    {
        if (Player.GetComponent<PlayerBody>().Hit(damage))
        {
            AudioSource.PlayClipAtPoint(hit, Player.transform.position, 1f);
        }
    }

    //... Footstep - Sound
    public void footstep(int _num)
    {
        sound.clip = footsounds[_num];
        sound.Play();
    }

    public override ActionState NotDead()
    {
        String chosen = ANIMS_MOVING[Math.Abs(new System.Random().Next() % ANIMS_MOVING.Length)];

        float speed = navMeshAgent.velocity.magnitude;
        //speed = (speed < 1f) ? 0f : (( speed - 1 ) * 2f);
        //Debug.Log("Speed of enemy: "+(speed));
        animator.SetFloat(chosen, speed);
        return null;
    }

    public override ActionState IsDying()
    {
        String chosen = ANIMS_DYING[Math.Abs(new System.Random().Next() % ANIMS_DYING.Length)];
        animator.SetBool(chosen, true);
        navMeshAgent.isStopped = true;
        PlayDeathSound();
        return null;
    }

    public override ActionState IsIdle()
    {
        ActionState state = null;
        /* Check if waypoint is set -> currentActionState == WALK* Else do idle*/
        //Debug.Log("IDLE");
        if (waypoints.Length > 0)
        {
            if (waypoints[waypointCount].x == 90000)
            {
                navMeshAgent.isStopped = true;
                //.. wait for 1 sec
                isIdleTimer += Time.deltaTime;
                if (isIdleTimer >= 1f)
                {
                    isIdleTimer = 0;
                    state = new Walk(patience);
                }
            }
            else if (waypoints[waypointCount].x == 100000)
            {
                //... wait for 10sec
                isIdleTimer += Time.deltaTime;
                if (isIdleTimer >= 10f)
                {
                    isIdleTimer = 0;
                    state = new Walk(patience);
                }
            }
            else if (waypoints[waypointCount].x == 150000)
            {
                //... wait for 5sec
                isIdleTimer += Time.deltaTime;
                if (isIdleTimer >= 5f)
                {
                    isIdleTimer = 0;
                    state = new Walk(patience);
                }
            }
            else if (waypoints[waypointCount].x == 200000)
            {
                //... switch to idle (for ever)
                state = new Idle(patience);
            }
            else //... go to the destination
            {
                navMeshAgent.isStopped = false;
                NavMeshHit navHit;
                NavMesh.SamplePosition(waypoints[waypointCount], out navHit, 20f, NavMesh.AllAreas);
                //NavMesh.
                //Debug.Log("SamplePosition: " + navHit.position.x + " / " + navHit.position.y + " / " + navHit.position.z);
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(navHit.position);
                //_DrawPath();
                state = new Walk(patience);
            }
        } // <= if waypoints are set! (...length>0)

        return state;
    }

    public override ActionState IsWalking()
    {
        if (waypoints.Length > 0 && currentSenseState != SenseStates.alarmed)
        {
            waypointCount++;
            waypointCount %= waypoints.Length;
        }

        return null;
    }

    public override ActionState IsChasing(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction = new Vector3(direction.x, direction.y * 0.5f, direction.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
        return null;
    }

    public override ActionState IsAttacking(bool attack)
    {
//        Debug.Log("Trying to attack " + gameObject.name);
        if (attack)
        {
//            Debug.Log("Trying to attack " + gameObject.name);
            // String chosen = ANIMS_ATTACKING[Math.Abs(new System.Random().Next()%ANIMS_ATTACKING.Length)];
            String chosen = ANIMS_ATTACKING[0];
//            Debug.Log("Choosen one: " + chosen + " " + gameObject.name);
            animator.SetBool(chosen, true);
        }
        else
        {
            //animator.GetCurrentAnimatorClipInfo()
            for (int i = 0; i < ANIMS_ATTACKING.Length; i++)
            {
                animator.SetBool(ANIMS_ATTACKING[i], false);
            }

            return new Search(0f);
        }

        return null;
    }

    public override ActionState IsLooking()
    {
        Vector3 center = gameObject.transform.position;
        NavMeshHit hit;
        float intensity = (currentSenseState == SenseStates.alarmed) ? 6 : 4;
        NavMesh.Raycast(
            center,
            center + gameObject.transform.forward * 6, out hit,
            NavMesh.AllAreas
        );
        float d = (hit.position - center).magnitude;
        d = (d > intensity) ? intensity : d;
        d = (intensity - d) / 3.5f;
        if (currentSenseState == SenseStates.attackedByPlayer)
        {
            //find the vector pointing from our position to the target
            Vector3 direction = (_lastSeenPlayerPosition - transform.position).normalized;
            //create the rotation we need to be in to look at the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * d);
        }
        else
        {
            transform.Rotate(0f, 120f * Time.deltaTime * d, 0f);
        }

        return null;
    }
}