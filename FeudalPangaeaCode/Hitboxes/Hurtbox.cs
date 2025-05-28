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

                    Vector3 ZPush = (GlobalPosition - node.GlobalPosition).Normalized();
                    Vector3 XPush = new Vector3(ZPush.Z, ZPush.Y, ZPush.X).Normalized();

                    GD.Print("ZPush: " + ZPush);

                    self.Push(Vector3.Up * hitbox.pushMod.Y + ZPush * hitbox.pushMod.Z);
                    break;
            }
        }
    }
}
