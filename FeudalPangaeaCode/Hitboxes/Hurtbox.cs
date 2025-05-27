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
        GD.Print("Hit Object: " + node.Name);

        if (node is Hitbox)
        {
            Hitbox hitbox = (Hitbox)node;

            self.ChangeHP(-hitbox.dmg, hitbox.damage_source);

            switch (hitbox.damage_source)
            {
                case DamageSource.Bonk:
                    self.Push((GlobalPosition - self.GetCreaturePosition()).Normalized() * hitbox.pushMod);
                    break;
            }
        }
    }
}
