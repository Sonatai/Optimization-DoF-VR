using UnityEngine.Video;

namespace Agent.Enemy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Serialization;
    using Random = UnityEngine.Random;

    public abstract class AbstractAI : MonoBehaviour
    {
        //... Public attributes
        public GameObject Flame;

        public Transform eyes;
        public LayerMask searchMask;
        public bool hasAlzheimer;
        public bool debug = false;

        //... Stats
        [FormerlySerializedAs("MS")] public int movementSpeed;
        public int maxHP;

        public float attackDistance = 1f;
        public int damage = 5;

        public float patience;
        public float currHP;

        public float alzheimerTimer = 0f;
        public GameObject Player;
        [FormerlySerializedAs("nav")] public NavMeshAgent navMeshAgent;
        public AudioClip death;
        public AudioClip hurt;
        [FormerlySerializedAs("sight")] public float sightDistance = 50;
        public float sightAngle = 66.666f;

        public GameObject AIRig;

        //... Private attributes
        protected Animator animator;
        protected AudioSource sound;
        protected Vector3 _lastRelativeHitPosition = new Vector3();
        protected Vector3 _lastSeenPlayerPosition = new Vector3();
        protected Vector3 _lastSearchPosition = new Vector3();


        private bool _flameOn;
        //private 

        public enum SenseStates
        {
            playerSpotted,
            attackedByPlayer,
            neverSeenPlayer,
            alarmed,
        }

        public SenseStates currentSenseState = SenseStates.neverSeenPlayer;

        protected ActionState _state = new Idle(20f);

        private void Start()
        {
            animator = GetComponent<Animator>();
            animator = animator == null ? GetComponentInChildren<Animator>() : animator;
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent = navMeshAgent == null ? GetComponentInChildren<NavMeshAgent>() : navMeshAgent;
            sound = GetComponent<AudioSource>();
            sound = sound == null ? GetComponentInChildren<AudioSource>() : sound;

            currHP = maxHP;
            navMeshAgent.speed = movementSpeed / 10f;
            //anim.speed = MS / 10f;
            alzheimerTimer = patience;
            //walkingTimer = patience;
            hasAlzheimer = false;
            _lastSearchPosition = gameObject.transform.position;
            navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            navMeshAgent.autoRepath = true;
            navMeshAgent.stoppingDistance = 0.1f;
            Construct();
        }


        //............................................................................
        public float GetAttackDistance()
        {
            return attackDistance;
        }

        protected virtual void Construct()
        {
        }

        public abstract ActionState NotDead();

        public abstract ActionState IsDying();
        public abstract ActionState IsIdle();

        public abstract ActionState IsWalking();

        public abstract ActionState IsChasing(Vector3 target);

        public abstract ActionState IsAttacking(bool attack);

        public abstract ActionState IsLooking();

        public void PlayDeathSound()
        {
            AudioSource.PlayClipAtPoint(death, gameObject.transform.position, 1f);
        }

        private void FixedUpdate()
        {
            //Debug.Log(currHP);
            //==============================================================================================================
            if (!(_state is Dead))
            {
                ActionState newState = NotDead();
                _state = (newState != null) ? newState : _state;
                if (currentSenseState != SenseStates.neverSeenPlayer)
                {
                    if (hasAlzheimer)
                    {
                        if (alzheimerTimer >= 0)
                        {
                            alzheimerTimer -= Time.deltaTime;
                        }
                        else
                        {
                            alzheimerTimer = patience;
                            currentSenseState = SenseStates.neverSeenPlayer;
                            _state = new Idle(20f);
                        }
                    }
                }


                var playerPosition = Player.transform.position;
                Vector3 vectorToPlayer = eyes.transform.position - playerPosition;

                float distanceFromPlayer = vectorToPlayer.magnitude;
                float angel = Vector3.Angle(-transform.forward, vectorToPlayer);

                //Debug.Log("distanceFromPlayer: " +distanceFromPlayer);
                if (distanceFromPlayer > 35)
                {
                    SwitchEnemyColliderState(false); //Improve performance
                }
                else
                {
                    SwitchEnemyColliderState(true);
                    if (
                        distanceFromPlayer <= 1.95
                        && !Physics.Linecast(eyes.position, Player.transform.position, searchMask)
                        || (distanceFromPlayer <= sightDistance
                            && angel <= sightAngle
                            && !Physics.Linecast(eyes.position, Player.transform.position, searchMask))
                    )
                    {
                        if (Mathf.Abs(eyes.position.y - playerPosition.y) < 3f) //Vertical distance is narrowed.
                        {
                            //...only when player is on the same level attacking makes sense!
                            _lastSeenPlayerPosition = Player.transform.position;
                            if (!(_state is Attack))
                            {
                                _state = new Chase(patience);
                                currentSenseState = SenseStates.playerSpotted;
                            }
                        }
                    }
                }
            }

            _state = _state.update(this, Time.deltaTime);
            _DrawPath();
        }

        private void SwitchEnemyColliderState(bool value)
        {
            if (AIRig)
            {
                AIRig.active = value;
                //animator.enabled = value;
            }
        }

        public void IsRoaming() //new roam position
        {
            Vector3 best = _BestTarget(gameObject.transform.position, 1);
            _lastSearchPosition = gameObject.transform.position;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(best);
        }


        private void _DrawPath()
        {
            if (debug)
            {
                NavMeshPath path = navMeshAgent.path;
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    Debug.DrawLine(
                        path.corners[i],
                        path.corners[i + 1],
                        Color.yellow
                    );
                    Debug.DrawLine(
                        path.corners[i] + new Vector3(0.0f, 3.0f, 0.0f),
                        path.corners[i + 1] + new Vector3(0.0f, 3.0f, 0.0f),
                        Color.yellow
                    );
                }

                Debug.DrawLine(
                    gameObject.transform.position + new Vector3(0.0f, 3.0f, 0.0f),
                    _lastSearchPosition + new Vector3(0.0f, 3.0f, 0.0f),
                    Color.blue
                );
            }
        }

        //... HP CALCULATION 
        public void Hit(float damage, Vector3 position)
        {
            if (!(_state is Dead))
            {
                sound.clip = hurt;
                sound.Play();
                currHP -= damage;
                _lastRelativeHitPosition =
                    position - gameObject.transform.position; //Vector from enemy center to hit position!
                if (currHP <= 0)
                {
                    IsDying();
                    _state = new Dead(2.5f);
                }
                else
                {
                    _lastSeenPlayerPosition = Player.transform.position;
                    currentSenseState = SenseStates.attackedByPlayer; //SenseStates.playerSpotted;
                    _state = new Search();
                }
            }
        }


        private bool PathIsClear(Vector3 source, Vector3 target)
        {
            Vector3 forward = target - source;
            RaycastHit[] hits = Physics.RaycastAll(source, forward, forward.magnitude - 0.25f, searchMask);
            //Debug.DrawLine(source, target, Color.blue);
            if (hits != null && hits.Length > 0)
            {
                //Debug.Log("Path is blocked!!!");
                return false;
            }

            //Debug.Log("Path is clear!!!");
            return true;
        }

        //... set _flameOn true or false
        public void SwitchFlameObjectVisible(bool value)
        {
            Flame.SetActive(value);
            _flameOn = value;
        }

        //... Getter for currHP
        public float GetCurrHP
        {
            get { return currHP; }
        }

        //... Getter for alive
        public bool GetAlive
        {
            get { return !(_state is Dead); }
        }

        public bool GetFlameOn
        {
            get { return _flameOn; }
        }

        protected Vector3 _BestTarget(Vector3 center, int recursion)
        {
            Vector3 oldDirection = _lastSearchPosition - center;
            if (oldDirection.magnitude > 0)
            {
                oldDirection /= oldDirection.magnitude;
            }

            NavMeshHit hit = new NavMeshHit();

            Vector3 target = center;
            Vector3 direction = Random.insideUnitSphere;
            direction = new Vector3(direction.x, 0.0f, direction.z);
            direction = direction / direction.magnitude;

            Vector3 best = center;

            int iterations = 26;
            float alpha = 360 / iterations;

            for (int i = 0; i < iterations; i++) //Checking out 24 directions! How far can the enemy go?
            {
                direction = Quaternion.Euler(0, -alpha, 0) * direction;
                target = center + direction * ((16 + new System.Random().Next() % 20));
                NavMesh.Raycast(center, target, out hit, NavMesh.AllAreas);
                float mod = 1f;
                if (oldDirection.magnitude > 0)
                {
                    mod = 1 - (1 + Vector3.Dot(oldDirection, direction)) / 2;
                }

                if ((center - best).magnitude < (center - hit.position).magnitude * mod)
                {
                    best = hit.position;
                }
            }

            if (recursion > 0) best = _BestTarget(best, recursion - 1);
            return best;
        }
    }
}