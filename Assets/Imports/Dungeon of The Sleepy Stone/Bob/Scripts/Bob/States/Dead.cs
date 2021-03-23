namespace Agent.Enemy
{
    using UnityEngine;

    public class Dead : ActionState
    {
        public Dead(float lifetime) : base(lifetime)
        {
        }

        public override ActionState update(AbstractAI ai, float delta)
        {
            //... DEAD ...///* Play Dead Animation  Despawn after x second*/
            _timer -= delta;
            if (_timer <= 0f)
            {
                //ai.PlayDeathSound();
                AbstractAI.Destroy(ai.gameObject);
            }

            return this;
        }
    }
}