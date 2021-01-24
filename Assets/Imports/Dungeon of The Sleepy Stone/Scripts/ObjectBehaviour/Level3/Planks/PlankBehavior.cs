using UnityEngine;

namespace ObjectBehaviour.Level3.Planks
{
    public class PlankBehavior : MonoBehaviour
    {
        void OnTriggerEnter(Collider collider) {
            //Debug.Log("Collider Plank");
            if (collider.CompareTag("crossbow"))
            {
                Crossbow bow = collider.GetComponentInChildren<Crossbow>();
                Vector3 force = (bow == null) ? collider.transform.forward : bow.DeltaVector;
                force /= Time.deltaTime;
                
                if (force.magnitude > 0.001f)
                {
                    Rigidbody[] fragments = GetComponentsInChildren<Rigidbody>();
                    for (int i = 0; i < fragments.Length; i++)
                    {
                        fragments[i].isKinematic = false;
                        fragments[i].AddForce(force * 5);
                    }
                }
            }
        }
    }
}