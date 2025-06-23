using Godot;

namespace MagicSystem
{
    public class FireballSpell : Spell
    {
        public override void Start()
        {
            caster.Dive();
        }

        public override void CastState(double delta)
        {
            caster.TryTransition(caster.GroundedCond(), CreatureState.Grounded);
            caster.TryTransition(caster.OpenAirCond(), CreatureState.OpenAir);
        }
    }
}