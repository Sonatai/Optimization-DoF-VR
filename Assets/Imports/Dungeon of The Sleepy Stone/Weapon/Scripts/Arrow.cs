using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{

    protected float l_damage;
    protected float h_damage;
    protected float m_damage;
	protected float g_damage;
    private float _count = 0f;
    [SerializeField]
    private float despawnCount = 60f;
    protected bool bounce = false;
    bool hit = false;
    [SerializeField]
    private LayerMask searchMask;
    [SerializeField]
    private AudioClip arrowHitSound;

    private AudioSource _audioSource;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        RaycastHit rayHit;
        if (Physics.Linecast(transform.position, transform.position + GetComponent<Rigidbody>().velocity * 0.02f, out rayHit, layerMask: searchMask) && !hit)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

            transform.position = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
            hit = true;
            _audioSource.PlayOneShot(arrowHitSound);
            if (rayHit.collider.attachedRigidbody != null)
            {
                this.transform.parent = rayHit.collider.transform;
            }
        }
    }

    private void Update()
    {
        _count += Time.deltaTime;
        if (_count > despawnCount)
        {
            Destroy(gameObject);
        }
    }

    protected void Damage(EnemyBehavior stat, Collider col)
    {
        stat.Hit(CalculateDamage(stat, col), gameObject.transform.position);
    }

    private float CalculateDamage(EnemyBehavior stat, Collider collider)
    {

        float damage = 
            stat.CompareTag("heavy") ? h_damage :
            stat.CompareTag("medium") ? m_damage :
            stat.CompareTag("light") ? l_damage :
            stat.CompareTag("ghul") ? g_damage :
            0;// <= represents immortality! (is the case when the enemy has tag 'immortal' (duh))
        
        return collider.CompareTag("Head") ? 1.5f * damage :
                collider.CompareTag("Body") ? damage :
                0; // <= represents for example: a shield! (is the case when the collider has tag 'immortal' (duh))
    }

    //Look for Collider
    private void OnTriggerEnter(Collider col)
    {
        //Look if Enemy Collider
        //If light,heavy immortal -> do different damage

        //Debug.Log("Trigger detaction in ~Arrow~ activated");
        
        EnemyBehavior stat = col.GetComponentInParent<EnemyBehavior>();
        if (stat != null)
        {
            Damage(stat, col);
        }
        
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = false;
        }
        gameObject.SetActive(true);
    }
}