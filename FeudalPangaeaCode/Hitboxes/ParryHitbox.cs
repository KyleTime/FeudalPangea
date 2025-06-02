using Godot;
using Godot.Collections;

public partial class ParryHitbox : Hurtbox
{
    [Export] public Node3D effect;

    public override void HitBy(Hitbox hitbox)
    {
        if (hitbox.Owner is not ICreature)
            return;

        ICreature creature = (ICreature)hitbox.Owner;

        creature.Stun(GlobalData.parryStunTime);

        if (effect != null)
        {
            effect.GlobalPosition = hitbox.GlobalPosition;
            effect.Visible = true;
        }
    }

    public bool OverlapsHitbox()
    {
        Array<Area3D> array = GetOverlappingAreas();

        foreach (Area3D area in array)
        {
            if (area.Owner != Owner && area is Hitbox)
            {
                return true;
            }
        }

        return false;
    }
}