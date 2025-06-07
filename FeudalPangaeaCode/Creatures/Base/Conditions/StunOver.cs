using Godot;

namespace CreatureBehaviors.CreatureConditions
{
    public class StunOver : BehaviorCondition
    {
        public bool Condition(CreatureStateMachine self)
        {
            return self.stunTimer == 0;
        }

        public ICreature GetTarget()
        {
            return null;
        }
    }
}