using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

namespace ObjectBehaviour.Level3
{
    public class BridgeBehavior : MonoBehaviour
    {
        private Animator animator;
        private bool trigger = false;
        private AudioSource sound;
        
        private float totalBurnTime = 8f;
        private float burnTime;
        private float maxLightIntensity;
        //Public Derialized 
        
        [Header("Inferno Settings")]
        public GameObject inferno;
        public ParticleSystem fire;
        public ParticleSystem sparkles;
        public Light light;

        [Header("Bridge Collider")] 
        [SerializeField]
        private NavMeshObstacle obstacle;
        [SerializeField] 
        private Collider bridgeColl;
        
        void Start()
        {
            burnTime = totalBurnTime;
            maxLightIntensity = light.intensity;
            light.range = 0.0f;
            light.intensity = 0.0f;
            animator = GetComponent<Animator>();
            sound = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (trigger)
            {
                burnTime -= Time.deltaTime;
                light.range = 20 - burnTime;
                float delta = burnTime / totalBurnTime;
                light.intensity = maxLightIntensity * (1 - delta) * (1 - delta) * delta * 15;
                if (burnTime < 0)
                {
                    trigger = false;
                } 
                else if (delta<0.3f)
                {
                    inferno.transform.position += Vector3.down * Time.deltaTime * 10 * (1 - delta / 0.3f);
                    animator.SetBool("isBurning", true);
                    //TODO: Test if enabling of colliders works
                    obstacle.enabled = true;
                    bridgeColl.enabled = true;
                }
            }
        }

        public void InflameWithOil()
        {
            inferno.SetActive(true);
            trigger = true;
            fire.Play();
            sparkles.Play();
            //Todo: Make enemies get fu%&§ed by this!
        }
        
        
        
        void OnTriggerStay(Collider col) {

            if (burnTime<2.15f)
            {
                EnemyBehavior enemy = col.GetComponentInParent<EnemyBehavior>();
                if (enemy)
                {
                    NavMeshAgent agent = col.GetComponentInParent<NavMeshAgent>();
                    agent.enabled = false;
                    Rigidbody body = col.GetComponentInParent<Rigidbody>();
                    body.constraints = RigidbodyConstraints.None;
                }
            }
        }
    }
}