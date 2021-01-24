namespace Agent.Enemy
{
    using UnityEngine;
    
    public class Search : ActionState
    {
        public Search(float lifetime) : base(lifetime) { }

        public Search() : base(3.25f){}

        public override ActionState update(AbstractAI ai, float delta)
        {
            ActionState state = this;
            if(_timer > 0f)  //... SEARCH ...//
            {
                    /*
                    * Check if player is seen -> currentActionState == WALK
                    * Check if Entity got hitted -> currentActionState = SEARCH, spottedPlayer = 3
                    * Check if playerSpotted == 0 -> currentActionState == IDLE
                    * Check if playerSpotted > 0 -> currentActionState == WALK
                    */
                    _timer -= delta;//... lookAroundTimer -> wie lang herumdrehen, lookAround, soll sich herum drehen
                    ai.navMeshAgent.isStopped = true;
                    //enemy.walkingTimer = _lifetime;//patience;//Transition!
                    ai.IsLooking();
            }
            else
            {
                if (ai.hasAlzheimer && ai.currentSenseState!=AbstractAI.SenseStates.neverSeenPlayer && ai.alzheimerTimer<0f)
                {
                    state = new Idle(20f);//Forgetful I guess
                }
                else 
                {
                    ai.IsRoaming();
                    //enemy.walkingTimer = _lifetime;//patience;//Transition!
                    state = new Walk(20f);
                    ai.currentSenseState = (ai.currentSenseState == AbstractAI.SenseStates.neverSeenPlayer) ? AbstractAI.SenseStates.neverSeenPlayer : AbstractAI.SenseStates.alarmed;
                }
                
            }
            return state;
        }
    }
}