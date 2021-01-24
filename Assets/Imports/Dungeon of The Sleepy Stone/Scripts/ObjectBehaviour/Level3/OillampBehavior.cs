using UnityEngine;

namespace ObjectBehaviour.Level3
{
    public class OillampBehavior : MonoBehaviour
    {
        private Animator animator;
        private AudioSource sound;
        private bool spilled = false;
        private float totalSpillTime = 5f;
        private float maxLightIntensity;
        
        private float spillTime = 0f;
        
        //Serialized:
        public GameObject bridge;
        public bool trigger = false;

        public GameObject spill;
        public ParticleSystem spillFlame;
        public Light light;
        
        void Start()
        {
            maxLightIntensity = light.intensity;
            spill.SetActive(false);
            animator = GetComponent<Animator>();
            sound = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (trigger)
            {
                Trigger();
            }
            if (spillTime<=0)
            {
                spill.SetActive(false);
            } 
            else if (spillTime>totalSpillTime)
            {
                spillTime -= Time.deltaTime;
                light.range = 20 - spillTime;
                float delta = spillTime / totalSpillTime;
                light.intensity = maxLightIntensity * delta;
            }
        }
        
        void OnTriggerEnter(Collider col) {
            
            if (col.CompareTag("Arrow") && !spilled)
            {
                Trigger();
            } 

        }


        private void Trigger()
        {
            spillTime = totalSpillTime;
            spill.SetActive(true);
            spillFlame.Play();
            trigger = false;
            animator.SetBool("isSpilling", true);
            BridgeBehavior bridgeBehavior = bridge.GetComponentInParent<BridgeBehavior>();
            bridgeBehavior.InflameWithOil();
            spilled = true;
        }
        
    }
}