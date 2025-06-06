using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export] public int dmg = 1;
    [Export] public Vector2 pushMod = new Vector2(10, 5);
    [Export] public DamageSource damage_source = DamageSource.Bonk;
    [Export] public CollisionShape3D collider;
    [Export] public float stunDuration = 1;

    [Signal]
    public delegate void HitHurtBoxEventHandler(Hurtbox hurtbox);

    public override void _Ready()
    {
        CollisionLayer = GlobalData.hitboxLayer;
        CollisionMask = GlobalData.hurtboxLayer;

        if (collider == null)
        {
            collider = GetNode<CollisionShape3D>("CollisionShape3D");
        }
    }

    public void Hit(Hurtbox hurtbox)
    {
        EmitSignal(SignalName.HitHurtBox, hurtbox);
    }
}
