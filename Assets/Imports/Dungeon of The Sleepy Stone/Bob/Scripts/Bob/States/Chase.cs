using JetBrains.Annotations;

namespace Agent.Enemy
{
    using UnityEngine;

    public class Chase : ActionState
    {
        public Chase(float lifetime) : base(lifetime)
        {
        }

        public override ActionState update(AbstractAI ai, float delta)
        {
            ai.navMeshAgent.speed = 1.25f * (ai.movementSpeed / 10f);
            ActionState state = this;
            /* Check if player is in range... Check if player is...  */
            //Debug.Log("CHASE");
            //... calculate the distance to the player

            ActionState newState = ai.IsChasing(ai.Player.transform.position);
            if (newState != null) return newState;

            Vector3 direction = ai.Player.transform.position - ai.transform.position;
            direction = new Vector3(direction.x, direction.y * 0.5f, direction.z);

            if (direction.magnitude > ai.GetAttackDistance())
            {
                //Debug.Log(direction.magnitude);
                ai.navMeshAgent.isStopped = false;
                ai.navMeshAgent.SetDestination(ai.Player.transform.position);
            }
            else
            {
                ai.navMeshAgent.isStopped = true;
                //enemy.playerFocusTimer = _lifetime;//patience;
                state = new Attack(20f);
            }

            if (_timer > 0f) //playerSpotted)//playerSpotted
            {
                _timer -= Time.deltaTime;
            }
            else if (_timer <= 0f
            ) // && currentSenseState!=SenseStates.playerSpotted)// && currentSenseState==SenseStates.alarmedplayerSpotted)
            {
                //enemy.playerFocusTimer = _lifetime;//patience;
                ai.currentSenseState = AbstractAI.SenseStates.alarmed;
                state = new Search(0f);
            }

            if (state != this)
            {
                ai.navMeshAgent.speed = ai.movementSpeed / 10f;
            }

            return state;
        }
    }
}