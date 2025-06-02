using Godot;
using System;

public partial class Hurtbox : Area3D
{
    public ICreature self;
    public bool hit { get; private set; }

    public override void _Ready()
    {
        if (Owner is ICreature)
        {
            self = (ICreature)Owner;
        }
        else
        {
            GD.PrintErr("Hurtbox not attached to valid ICreature! Attached to: " + Owner.Name);
        }

        AreaEntered += OnAreaEntered;
        CollisionLayer = GlobalData.hitboxLayer;
        CollisionMask = GlobalData.hurtboxLayer;
    }

    public override void _PhysicsProcess(double delta)
    {
        hit = false;
    }  

    public void OnAreaEntered(Area3D node)
    {
        GD.Print("Hit Object: " + Owner.Name);

        if (node is Hitbox)
        {
            Hitbox hitbox = (Hitbox)node;

            if (hitbox.Owner == Owner)
                return;

            HitBy(hitbox);
        }
    }

    public virtual void HitBy(Hitbox hitbox)
    {
        hit = true;

        self.ChangeHP(-hitbox.dmg, hitbox.damage_source);

        hitbox.Hit(this); //let hitbox know it hit something

        switch (hitbox.damage_source)
        {
            case DamageSource.Bonk:

                Vector3 ZPush = ((GlobalPosition - hitbox.GlobalPosition) with { Y = 0 }).Normalized();

                self.Push(Vector3.Up * hitbox.pushMod.Y + ZPush * hitbox.pushMod.Z);
                self.Stun(hitbox.stunDuration);
                break;
        }
    }
}
