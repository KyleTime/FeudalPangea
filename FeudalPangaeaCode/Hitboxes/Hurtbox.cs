using Godot;
using System;
using System.Collections.Generic;

public partial class Hurtbox : Area3D
{
    public ICreature self;
    public bool hit { get; private set; }

    [Export] public CollisionShape3D collider;
    [Export] public HitFx bonkEffect;

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
        CollisionLayer = GlobalData.hurtboxLayer;
        CollisionMask = GlobalData.hitboxLayer;

        if (collider == null)
        {
            collider = GetNode<CollisionShape3D>("CollisionShape3D");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        SetDeferred("hit", false);
    }  

    public void OnAreaEntered(Area3D node)
    {
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

                Vector3 pushVector = CreatureVelocityCalculations.PushVector(hitbox.GlobalPosition, self.GetCreatureCenter(), hitbox.pushMod.X) with {Y = 0};

                self.Push(pushVector + Vector3.Up * hitbox.pushMod.Y);

                if (hitbox.stunDuration > 0)
                    self.Stun(hitbox.stunDuration);

                if (bonkEffect != null)
                {
                    bonkEffect.Effect(hitbox.GlobalPosition, hitbox.Rotation);
                }
                    
                break;
        }
    }
}
