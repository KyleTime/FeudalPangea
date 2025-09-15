using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class DeathQueueFree : BehaviorState
    {
        public DeathQueueFree() : base(CreatureState.Dead)
        {
        }

        public override void HandleAnimation(AnimationPlayer player)
        {  
        }

        public override Vector3 TransitionIn(CreatureStateMachine self, double delta)
        {
            self.QueueFree();
            return new Vector3();
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            return new Vector3();
        }
    }
}