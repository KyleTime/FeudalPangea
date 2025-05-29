using Godot;
using System;

namespace CreatureBehaviors.CreatureStates
{
    public class FollowCreature : BehaviorState
    {
        float acceleration;
        float deceleration;
        float maxSpeed;
        float jumpPower;

        public FollowCreature(float acceleration, float deceleration, float maxSpeed, float jumpPower) : base(CreatureState.Grounded)
        {
            SetUp(acceleration, deceleration, maxSpeed, jumpPower);
        }

        public FollowCreature(float acceleration, float deceleration, float maxSpeed) : base(CreatureState.Grounded)
        {
            SetUp(acceleration, deceleration, maxSpeed, 0);
        }

        private void SetUp(float acceleration, float deceleration, float maxSpeed, float jumpPower)
        {
            this.acceleration = acceleration;
            this.deceleration = deceleration;
            this.maxSpeed = maxSpeed;
            this.jumpPower = jumpPower;
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            if (self.target == null)
            {
                GD.Print("HELPPPPP");
                return self.GetCreatureVelocity();
            }

            Vector3 velocity = self.GetCreatureVelocity();

            self.LookAt(Player.player.GlobalPosition);
            self.Rotation = new Vector3(0,self.Rotation.Y, 0);

            velocity = CreatureVelocityCalculations.Accelerate(velocity, acceleration, deceleration, maxSpeed, self.target.GetCreaturePosition() - self.GlobalPosition, delta);

            velocity.Y = CreatureVelocityCalculations.Gravity(velocity.Y, (float)delta);

            return velocity;
        }
    }
}