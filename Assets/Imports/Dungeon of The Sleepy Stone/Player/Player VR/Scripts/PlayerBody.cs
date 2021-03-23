using Player;
using UnityEngine;


public class PlayerBody : MonoBehaviour
{
    public float health;

    //public int deff;
    bool isAlive = true;
    HeartbeatController heartbeatController;

    //... HP CALCULATION ...//
    private void Awake()
    {
        heartbeatController = GetComponent<HeartbeatController>();
    }

    private void FixedUpdate()
    {
        if (health < 50)
        {
            health += 0.02f;
            heartbeatController.UpdateHp(health);
        }
    }

    public bool Hit(int damage)
    {
        if (!isAlive) //no need to execute when already dead, also hit sound wont be played then
        {
            return false;
        }

        health -= damage;
        heartbeatController.UpdateHp(health);
        if (health <= 0)
        {
            isAlive = false;
            (FindObjectOfType(typeof(FadingScript)) as FadingScript).Death();
        }

        return true;
    }
}