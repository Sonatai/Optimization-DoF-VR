using UnityEngine;


namespace ObjectBehaviour.Furniture
{
    public class ChestBehavior : Triggerable
    {
        private AudioSource sound;
        public Animator animator;
        
        //Serialized:
        public bool trigger = false;

        public override void Trigger(HandController hand)
        {
            animator.SetBool("Swap", true);
        }

    }
}