namespace Agent.Enemy
{
    public class Walk : ActionState
    {
        public Walk(float lifetime) : base(lifetime)
        {
        }

        public override ActionState update(AbstractAI ai, float delta)
        {
            ActionState state = this;
            /* Check if player isn't seen anymore -> currentActionState == SEARCH
            * Check if waypoint reached -> waypoint index++ , currentActionState == IDLE
            */
            if (ai.currentSenseState == AbstractAI.SenseStates.playerSpotted) //playerSpotted)
            {
                state = new Search();
            }

            if (_timer > 0)
            {
                //... check if npc reach destination
                if (ai.navMeshAgent.remainingDistance <= ai.navMeshAgent.stoppingDistance &&
                    !ai.navMeshAgent.pathPending)
                {
                    ai.IsWalking();
                    _timer = 0.0f;
                    //STOP WALKING WHEN POINT REACHED!
                }
            }
            else if (ai.currentSenseState != AbstractAI.SenseStates.neverSeenPlayer) //Time ran out: next state:
            {
                state = new Search();
            }
            else
            {
                state = new Idle(20);
            }

            _timer -= delta;
            return state;
        }
    }
}