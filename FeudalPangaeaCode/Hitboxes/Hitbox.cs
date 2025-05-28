using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export] public int dmg = 1;
    [Export] public Vector3 pushMod = new Vector3(0, 5, 10);
    [Export] public DamageSource damage_source = DamageSource.Bonk;
    [Export] public CollisionShape3D collider;
    [Export] public float stunDuration = 1;

    public override void _Ready()
    {
        CollisionLayer = 16;
        CollisionMask = 0;

        if (collider == null)
        {
            collider = GetNode<CollisionShape3D>("CollisionShape3D");
        }
    }
}
