using Godot;

namespace CreatureBehaviors.CreatureConditions
{
    public class HitCreature : BehaviorCondition
    {
        private ICreature target;
        private bool hitSomething = false;

        public HitCreature(Hitbox[] hitboxes)
        {
            foreach (Hitbox h in hitboxes)
            {
                h.HitHurtBox += HitDetected;
            }
        }

        public void HitDetected(Hurtbox hurtbox)
        {
            GD.Print("HIT DETECTED");

            if (hitSomething)
                return;

            target = hurtbox.self;
            hitSomething = true;
        }

        public bool Condition(CreatureStateMachine self)
        {
            bool hit = hitSomething;
            hitSomething = false;
            return hit;
        }

        public ICreature GetTarget()
        {
            return target;
        }
    }
}