namespace Agent.Enemy
{
    using UnityEngine;
    
    public class Attack : ActionState
    {
        public Attack(float lifetime) : base(lifetime)
        { }

        public override ActionState update(AbstractAI ai, float delta)
        {
            //Debug.Log("ATTACK");
            Vector3 direction = ai.Player.transform.position - ai.transform.position;
            direction = new Vector3(direction.x, direction.y*0.5f, direction.z);
            ai.transform.rotation = Quaternion.Slerp(ai.transform.rotation, Quaternion.LookRotation(direction), 0.1f);
            
            ActionState state = null;
            state = ai.IsAttacking(direction.magnitude <= ai.GetAttackDistance());
            if (state!=null) {
                return state;
            }
            return this;
        }
    }
}