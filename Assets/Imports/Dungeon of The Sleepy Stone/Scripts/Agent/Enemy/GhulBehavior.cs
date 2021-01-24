using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Agent.Enemy;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GhulBehavior : AbstractAI
{
    //private float patience = 10f;

    //private Vector3 lastSearchPosition = new Vector3();

    private Vector3[] _pathPoints = null;
    private float _jumpSpeed = 0f;
    private int _currentPathIndex = 0;
    private GameObject _slave;
    private RigidbodyConstraints _agentConstraints;
    private float _jumpPathDistance = 0;

    protected override void Construct()
    {
        _slave = GameObject.Find("GhulRig");
        Rigidbody body = GetComponentInChildren<Rigidbody>();
        _agentConstraints = body.constraints;
    }

    public String[] ANIMS_MOVING = {"isMoving"};
    public String[] ANIMS_ATTACKING = {"isAttacking1", "isAttacking2"};
    public String[] ANIMS_DYING = {"isDying1", "isDying2"};

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
        animator.SetFloat("isJumpAttacking", 1+_jumpSpeed);
        String chosen = ANIMS_MOVING[Math.Abs(new System.Random().Next() % ANIMS_MOVING.Length)];

        float speed = navMeshAgent.velocity.magnitude;
        //speed = (speed < 1f) ? 0f : (( speed - 1 ) * 2f);
        //Debug.Log("Speed of enemy: "+(speed));
        animator.SetFloat(chosen, speed);
        
        if(_pathPoints!=null)
        {
            //Move on path
            if (_currentPathIndex < _pathPoints.Length)
            {
                Vector3 moveVec = _pathPoints[_currentPathIndex] - _slave.transform.position;
                if (debug) Debug.DrawLine(_slave.transform.position, _pathPoints[_currentPathIndex], Color.white);
                if (moveVec.magnitude > 0.3f)
                {
                    moveVec = moveVec * _jumpSpeed / moveVec.magnitude;
                    _slave.transform.position += moveVec;
                    _slave.transform.rotation = gameObject.transform.rotation;
                }
                else
                {
                    _currentPathIndex++;
                }
                Vector3 groundPoint = new Vector3(
                    _slave.transform.position.x, 0, _slave.transform.position.z
                 ); 
                float startDistance = (_pathPoints[0] - groundPoint).magnitude;
                float endDistance = (_pathPoints[_pathPoints.Length-1] - groundPoint).magnitude;
                
                _jumpSpeed = (startDistance / _jumpPathDistance) * (endDistance / _jumpPathDistance);
                //Debug.Log("Speed: "+_jumpSpeed);
                _jumpSpeed *= Mathf.Pow(1f + _jumpSpeed, 100);
                _jumpSpeed = (_jumpSpeed > 0.1f) ? 0.1f : _jumpSpeed;
                
            }
            else
            {
                //Hit();
                _currentPathIndex = 0;
                _jumpSpeed = 0;
                //if (_pathPoints.Length > 0)
                //{
                    gameObject.transform.position = _pathPoints[_pathPoints.Length-1];
                    _slave.transform.position = _pathPoints[_pathPoints.Length-1];
                //}
                //navMeshAgent.enabled = true;
                Rigidbody body = GetComponentInChildren<Rigidbody>();
                body.constraints = _agentConstraints;
                _pathPoints = null;
                movementSpeed = (int) (movementSpeed*1.5f);
                movementSpeed = (movementSpeed > 100) ? 100 : movementSpeed;
                return new Search(0f);
            }
            return new Idle(1);
        }
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
        if (_pathPoints!=null)
        {
            return new Idle(1f);
        }
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
        ActionState state = null;
        if (_pathPoints == null)
        {
            //Rotating towards player:
            Vector3 direction = target - transform.position;
            direction = new Vector3(direction.x, direction.y * 0.5f, direction.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            
            //Test if jump attack is possible:
            Vector3 self = gameObject.transform.position;
            Vector3 victim = Player.transform.position+new Vector3(0f, 0.5f, 0f);
            Vector3 attackVec = victim - self;
            float distanceToVictim = attackVec.magnitude;
            if (debug) Debug.DrawLine(self, victim, Color.blue);

            if (distanceToVictim<8.25f && distanceToVictim>7.25f)
            {
                attackVec = new Vector3(victim.x, 0, victim.z) - self;
                Vector3 landingSpot = self + (2.1f * attackVec);
                if (debug) Debug.DrawLine(victim, landingSpot, Color.white);
                
                NavMeshHit hit;
                NavMesh.Raycast(
                    self,
                    landingSpot, out hit,
                    NavMesh.AllAreas
                );
                float d = (hit.position - landingSpot).magnitude;
                landingSpot = self + (1.9f * attackVec);//
                if (d < 0.1f) //Jump attack possible
                {
                    animator.SetFloat("isJumpAttacking", 1+_jumpSpeed);
                    Vector3 landVec = landingSpot - victim;
                    float travelDistance = distanceToVictim + landVec.magnitude;
                    _pathPoints = new[] {self, victim, landingSpot};
                    _jumpSpeed = travelDistance / 100000f;
                    _jumpPathDistance = travelDistance;
                    navMeshAgent.isStopped = true;
                    Rigidbody body = GetComponentInChildren<Rigidbody>();
                    _agentConstraints = body.constraints;
                    //body.constraints = RigidbodyConstraints.None;
                } 
            }
        }
        else
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.SetDestination(gameObject.transform.position);
            return new Idle(1f);
        }
        return state;
    }

    private float attackOnceCounter = 1.5f;
    
    public override ActionState IsAttacking(bool attack)
    {
        // Debug.Log("Trying to attack " + gameObject.name);
        if (attack)
        {
            // Debug.Log("Trying to attack " + gameObject.name);
            // String chosen = ANIMS_ATTACKING[Math.Abs(new System.Random().Next()%ANIMS_ATTACKING.Length)];
            String chosen = ANIMS_ATTACKING[0];
            // Debug.Log("Choosen one: " + chosen + " " + gameObject.name);
            animator.SetBool(chosen, true);
            attackOnceCounter -= Time.deltaTime;

            if (attackOnceCounter < 0)
            {
                attackOnceCounter = 1.5f;
                Hit();
                
            }
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