using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class Idle : BehaviorState
    {
        public bool doGravity;
        public float deceleration;

        public Idle(bool doGravity, float deceleration = 5) : base(CreatureState.Grounded)
        {
            this.doGravity = doGravity;
            this.deceleration = deceleration;
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            Vector3 velocity = self.GetCreatureVelocity();

            if (doGravity)
            {
                velocity.Y = CreatureVelocityCalculations.Gravity(velocity.Y, delta);
            }

            velocity = CreatureVelocityCalculations.Decelerate(velocity, deceleration, delta);

            return velocity;
        }
    }
}