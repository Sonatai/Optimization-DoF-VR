using System.Timers;

namespace Agent.Enemy
{
    using UnityEngine;

    public abstract class ActionState
    {
        protected float _lifetime;

        protected float _timer;

        public ActionState(float lifetime)
        {
            _lifetime = lifetime;
            _timer = _lifetime;
        }

        public abstract ActionState update(AbstractAI ai, float delta);
    }
}