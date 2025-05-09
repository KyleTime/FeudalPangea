using Godot;

public class DoubleJumpSpell : Spell {
    public DoubleJumpSpell(PlayerMovement caster) : base(caster)
    {
    }

    public override void CastState(double delta)
    {
        caster.velocity += new Vector3(0, 100, 0);

        caster.TryTransition(caster.GroundedCond(), CreatureState.Grounded);
	    caster.TryTransition(caster.OpenAirCond(), CreatureState.OpenAir);
    }
}