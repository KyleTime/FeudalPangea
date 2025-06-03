using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class ParryHitbox : Hurtbox
{
    [Export] public Node3D effect;
    [Export] public float horizontalPush = 1;
    [Export] public float verticalPush = 1;

    public override void HitBy(Hitbox hitbox)
    {
        if (hitbox.Owner is not ICreature)
            return;

        ICreature creature = (ICreature)hitbox.Owner;

        creature.Stun(GlobalData.parryStunTime);

        CreatureVelocityCalculations.PushCreature(self, creature, horizontalPush);
        creature.Push(Vector3.Up * verticalPush);

        GD.Print("PARRY!");

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
            if (area.Owner != Owner && area is Hitbox && area.Owner is ICreature)
            {
                return true;
            }
        }

        return false;
    }
}