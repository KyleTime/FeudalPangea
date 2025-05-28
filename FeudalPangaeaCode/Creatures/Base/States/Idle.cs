using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class Idle : BehaviorState
    {
        public bool doGravity;

        public Idle(bool doGravity) : base(CreatureState.Grounded)
        {
            this.doGravity = doGravity;
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            Vector3 velocity = self.GetCreatureVelocity();

            if (doGravity)
            {
                velocity.Y = CreatureVelocityCalculations.Gravity(velocity.Y, delta);
            }

            velocity = CreatureVelocityCalculations.Decelerate(velocity, 3, delta);

            return velocity;
        }
    }
}