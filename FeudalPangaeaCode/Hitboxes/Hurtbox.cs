using Godot;
using System;
using System.Collections.Generic;

public partial class Hurtbox : Area3D
{
    public ICreature self;
    public bool hit { get; private set; }

    [Export] public CollisionShape3D collider;
    [Export] public HitFx bonkEffect;
    [Export] public HitFx dinkEffect;

    bool isPlayer = false;

    private readonly DamageSource[] majorCreatureVulnerabilities = [DamageSource.Fall, DamageSource.Fire, DamageSource.Magic];

    public override void _Ready()
    {
        if (Owner is ICreature)
        {
            self = (ICreature)Owner;

            isPlayer = self is Player;
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
        hitbox.Hit(this); //let hitbox know it hit something

        //I know the ordering looks weird, but it's a lil
        //necessary for hitboxes to know they're touching the player
        //even if no damage is being dealt
        if (!hitbox.hitsPlayer && isPlayer || hitbox.ignore != null && hitbox.ignore == Owner)
        {
            return;
        }

        hit = true;

        //if major and not stunned, resist nonmagical damage
        if (self.IsMajor && !(self.GetState() == CreatureState.Stun || self.GetState() == CreatureState.StunAir))
        {
            foreach (DamageSource dmg in majorCreatureVulnerabilities)
            {
                if (dmg == hitbox.damage_source)
                {
                    Vector3 pushVector = CreatureVelocityCalculations.PushVector(hitbox.GlobalPosition, self.GetCreatureCenter(), hitbox.pushMod.X) with { Y = 0 };

                    self.Push(pushVector.Normalized() * 10f);

                    if (dinkEffect != null)
                    {
                        dinkEffect.Effect(hitbox.GlobalPosition, hitbox.Rotation);
                    }
                    return;
                }
            }
        }

        self.ChangeHP(-hitbox.dmg, hitbox.damage_source);

        switch (hitbox.damage_source)
        {
            case DamageSource.Bonk:
            case DamageSource.Magic:
            case DamageSource.Fire:

                Vector3 pushVector = CreatureVelocityCalculations.PushVector(hitbox.GlobalPosition, self.GetCreatureCenter(), hitbox.pushMod.X) with { Y = 0 };

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
