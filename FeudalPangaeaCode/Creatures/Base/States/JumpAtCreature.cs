using System;
using Godot;


namespace CreatureBehaviors.CreatureStates {
    public class JumpAtCreature : BehaviorState
    {
        public Hitbox[] hitboxes;
        public float delay; //delay before jump
        float timer = 0;
        public float forwardVelocity; //how fast towards the target should the creature jump
        public float upwardVelocity; //how high should the creature jump

        public float deceleration;

        bool jump = false;
        bool stop = false; //when the hitboxes hit something, this should be true and GetStepVelocity should deal with it
        bool waiting = false; //when this is true, behavior becomes essentially the same as idle

        public JumpAtCreature(Hitbox[] hitboxes, float delay = 1, float forwardVelocity = 10, float upwardVelocity = 5, float deceleration = 10) : base(CreatureState.Attack)
        {
            this.hitboxes = hitboxes;

            foreach (Hitbox h in hitboxes)
            {
                h.HitHurtBox += StopSelf;
            }

            this.forwardVelocity = forwardVelocity;
            this.upwardVelocity = upwardVelocity;
            this.deceleration = deceleration;
        }

        public JumpAtCreature(Hitbox hitbox, float delay = 1, float forwardVelocity = 10, float upwardVelocity = 5, float deceleration = 10) : base(CreatureState.Attack)
        {
            this.hitboxes = [hitbox];
            hitbox.HitHurtBox += StopSelf;

            this.forwardVelocity = forwardVelocity;
            this.upwardVelocity = upwardVelocity;
            this.deceleration = deceleration;
        }

        public override void HandleAnimation(AnimationPlayer player)
        {
        }

        public override Vector3 TransitionOut(CreatureStateMachine self, double delta)
        {
            Vector3 vel = self.Velocity;

            vel = CreatureVelocityCalculations.Decelerate(vel, deceleration * 2, delta);
            vel *= new Vector3(-1, 0, -1);
            waiting = true;

            foreach (Hitbox h in hitboxes)
            {
                h.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
            }

            return vel;
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            Vector3 vel = self.GetCreatureVelocity();

            //apply gravity
            vel.Y = CreatureVelocityCalculations.Gravity(vel.Y, delta);

            vel = CreatureVelocityCalculations.Decelerate(vel, deceleration, delta);

            if (waiting)
            {
                return vel;
            }

            if (timer <= delay)
            {
                timer += (float)delta;
                return self.GetCreatureVelocity();
            }

            if (!jump)
            {
                Vector3 jumpVel = ((self.target.GetCreaturePosition() - self.GetCreaturePosition()).Normalized() with { Y = 0 } * forwardVelocity) + Vector3.Up * upwardVelocity;
                jump = true;

                foreach (Hitbox h in hitboxes)
                {
                    h.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
                }

                return vel + jumpVel;
            }

            if (stop)
            {
                vel = CreatureVelocityCalculations.Decelerate(vel, deceleration * 2, delta);
                vel *= new Vector3(-1, 0, -1);
                waiting = true;

                foreach (Hitbox h in hitboxes)
                {
                    h.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
                }
            }

            return vel;
        }

        //called whenever any of the hitboxes collide with something
        public void StopSelf(Hurtbox hurtbox)
        {
            GD.Print("HIT!");

            stop = true;
        }
    }

}