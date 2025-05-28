using Godot;
using System;

public partial class Hurtbox : Area3D
{
    public ICreature self;

    public override void _Ready()
    {
        if (Owner is ICreature)
        {
            self = (ICreature)Owner;
        }
        else
        {
            GD.PrintErr("Hurtbox not attached to valid ICreature!");
        }

        AreaEntered += OnAreaEntered;
        CollisionLayer = 0;
        CollisionMask = 16;
    }

    public void OnAreaEntered(Area3D node)
    {
        if (node is Hitbox)
        {
            Hitbox hitbox = (Hitbox)node;

            if (hitbox.Owner == Owner)
                return;

            self.ChangeHP(-hitbox.dmg, hitbox.damage_source);

            switch (hitbox.damage_source)
            {
                case DamageSource.Bonk:

                    Vector3 ZPush = ((GlobalPosition - node.GlobalPosition) with { Y = 0 }).Normalized();

                    self.Push(Vector3.Up * hitbox.pushMod.Y + ZPush * hitbox.pushMod.Z);
                    self.Stun(hitbox.stunDuration);
                    break;
            }
        }
    }
}
