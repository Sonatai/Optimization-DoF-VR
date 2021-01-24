using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowFlaming : Arrow
{
    protected float dot;
    protected bool isHit = false;
    private bool isLighted = false;
    [SerializeField]
    private GameObject Flame;
    private bool collided = false;
    private float ghulMultiplier = 5f;

    public ArrowFlaming()
    {
        l_damage = 1;
        h_damage = 1;
        m_damage = 1;
        g_damage = 5;
        dot = 1;
        
    }

    private void Start()
    {
        isLighted = true;
        Flame.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyBehavior enemyBehavior = other.GetComponentInParent<EnemyBehavior>();
        
        //Check if Enemy Collider
        if (enemyBehavior && !collided)
        {
            Damage(enemyBehavior, other);
            
            //Dot Damage
            if (!enemyBehavior.GetFlameOn && !isHit)
            {
                isHit = true;
                
                float burningDamageInterval =
                    enemyBehavior.CompareTag("light") ? 1f :
                    enemyBehavior.CompareTag("medium") ? 1.5f :
                    enemyBehavior.CompareTag("heavy") ? 2f :
                    enemyBehavior.CompareTag("ghul") ? 5f :
                    100000; // if enemyBehavior.CompareTag("Immortal") wait froever
                
                StartCoroutine(setEnemyOnFire(dot, enemyBehavior, burningDamageInterval));
            }

            foreach (Collider arrowCollider in GetComponents<Collider>())
            {
                arrowCollider.enabled = false;
            }            
        }
        else if (other.gameObject.CompareTag("ExplosiveBarrel") && !collided)
        {
            ExplodeBarrelController explodeBarrelController = other.gameObject.GetComponent<ExplodeBarrelController>();
            
            explodeBarrelController.Explode();
            Destroy(gameObject);

        }
        
        collided = true;
    }
    IEnumerator setEnemyOnFire(float damage, EnemyBehavior enemyBehavior, float burningDamageInterval)
    {
        if (burningDamageInterval == 100000)
        {
            if (enemyBehavior)
            {
                enemyBehavior.SwitchFlameObjectVisible(false);
                gameObject.SetActive(true);
            }
        }
        enemyBehavior.SwitchFlameObjectVisible(true);
        
        for (int damageCount = 0; damageCount < 10; damageCount++)
        {
            if (enemyBehavior.CompareTag("ghul"))
            {
                enemyBehavior.Hit(ghulMultiplier* damage, gameObject.transform.position);
            }
            else
            {
                enemyBehavior.Hit(damage, gameObject.transform.position);
            }
            
            yield return new WaitForSeconds(burningDamageInterval);
            damageCount++;
        }

        //checks if enemy is already dead (destroyed)
        if (enemyBehavior)
        {
            enemyBehavior.SwitchFlameObjectVisible(false);
            gameObject.SetActive(true);
        }
    }
}