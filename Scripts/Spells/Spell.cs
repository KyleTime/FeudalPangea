using Godot;

public abstract class Spell {

    protected PlayerMovement caster;

    public Spell(PlayerMovement caster){
        this.caster = caster;
    }

    public abstract void CastState(double delta);
}