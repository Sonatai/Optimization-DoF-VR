using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
 * Reduces Health Points of Player
 */
public class EnemyMeleeHit : MonoBehaviour {
    public int damage;
    public EnemyBehavior myStats;

    /*void OnTriggerEnter(Collider col) {
        if (col.GetComponent<PlayerBody>()) {
            if (myStats.GetAlive) {
                col.gameObject.GetComponent<PlayerBody>().Hit(damage);
                GetComponent<AudioSource>().Play();
                //Debug.Log("Pawwww");
            }
        }
    }*/
}
