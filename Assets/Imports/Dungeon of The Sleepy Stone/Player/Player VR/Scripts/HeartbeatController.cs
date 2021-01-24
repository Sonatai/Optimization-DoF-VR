using System;
using UnityEngine;

namespace Player
{
    public class HeartbeatController : MonoBehaviour
    {
        public AudioClip[] heartbeats;
        AudioSource source;
        private int current = 10;

        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        public void UpdateHp(float health)
        {
            int x = (int)(health * 0.2);
            if (current != x && x > 0)
            {
                if (x >= 10)
                {
                    source.loop = false;
                }
                else
                {
                    source.clip = heartbeats[x];
                    source.loop = true;
                    source.Play();
                }
                current = x;
            }
        }
    }
}