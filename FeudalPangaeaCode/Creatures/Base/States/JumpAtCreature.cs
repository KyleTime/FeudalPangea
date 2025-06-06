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

        public float delayBeforeJump;
        public float endLag;

        bool jump = false;
        bool stop = false; //when the hitboxes hit something, this should be true and GetStepVelocity should deal with it
        bool waiting = false; //when this is true, behavior becomes essentially the same as idle

        public JumpAtCreature(Hitbox[] hitboxes, float forwardVelocity = 10, float upwardVelocity = 5, float deceleration = 10, float delayBeforeJump = 1.5f, float endLag = 2f) : base(CreatureState.Attack)
        {
            this.hitboxes = hitboxes;

            foreach (Hitbox h in hitboxes)
            {
                h.HitHurtBox += StopSelf;
            }

            this.forwardVelocity = forwardVelocity;
            this.upwardVelocity = upwardVelocity;
            this.deceleration = deceleration;
            this.delayBeforeJump = delayBeforeJump;
            this.endLag = endLag;
        }

        public override void HandleAnimation(AnimationPlayer player)
        {
        }

        public override Vector3 TransitionIn(CreatureStateMachine self, double delta)
        {
            stop = false;
            timer = 0;
            waiting = false;
            jump = false;
            holdTransitions = true;

            return self.Velocity;
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
                holdTransitions = false;
                return vel;
            }
            
            timer += (float)delta;

            if (!jump && timer > delayBeforeJump)
            {
                Vector3 jumpVel = ((-self.Basis.Z with {Y = 0}).Normalized() with { Y = 0 } * forwardVelocity) + Vector3.Up * upwardVelocity;
                jump = true;
                timer = 0;

                foreach (Hitbox h in hitboxes)
                {
                    h.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
                }

                return vel + jumpVel;
            }
            
            if(stop || jump && timer > endLag)
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
            stop = true;
        }
    }

}