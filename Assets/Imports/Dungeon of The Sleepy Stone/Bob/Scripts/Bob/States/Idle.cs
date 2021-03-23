namespace Agent.Enemy
{
    public class Idle : ActionState
    {
        public Idle(float lifetime) : base(lifetime)
        {
        }

        public override ActionState update(AbstractAI ai, float delta)
        {
            ActionState state = ai.IsIdle();
            if (state == null) return this;
            if (ai.currentSenseState != AbstractAI.SenseStates.neverSeenPlayer)
            {
                _lifetime = (ai.currentSenseState == AbstractAI.SenseStates.playerSpotted) ? 0 : _lifetime;
                _lifetime -= delta;
                if (_lifetime < 0)
                {
                    return new Search();
                }
            }

            return state;
        }
    }
}