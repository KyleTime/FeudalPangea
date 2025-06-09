using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class Idle : BehaviorState
    {
        public bool doGravity;
        public float deceleration;
        public float holdTime; //amount of time this state will wait before attempting to transition
        float timer = 0;

        public Idle(bool doGravity, float deceleration = 5, float holdTime = 0, CreatureState stateOverride = CreatureState.Grounded) : base(stateOverride)
        {
            this.doGravity = doGravity;
            this.deceleration = deceleration;
            this.holdTime = holdTime;
            holdTransitions = true;
        }

        public override void HandleAnimation(AnimationPlayer player)
        {
        }

        public override Vector3 TransitionIn(CreatureStateMachine self, double delta)
        {
            timer = 0;
            holdTransitions = true;

            return base.TransitionIn(self, delta);
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            Vector3 velocity = self.GetCreatureVelocity();

            if (doGravity)
            {
                velocity.Y = CreatureVelocityCalculations.Gravity(velocity.Y, delta);
            }

            timer += (float)delta; //if no transition happens for like, some absurd length of time, this will overflow, I am not worried

            if (timer > holdTime)
            {
                holdTransitions = false;
            }

            velocity = CreatureVelocityCalculations.Decelerate(velocity, deceleration, delta);

            return velocity;
        }
    }
}