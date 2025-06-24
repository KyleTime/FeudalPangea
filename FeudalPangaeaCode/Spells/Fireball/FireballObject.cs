using Godot;
using System;
using System.Collections;

public partial class FireballObject : Node3D
{
    public Vector3 velocity;
    [Export] Area3D groundbox;
    [Export] Hitbox hitbox;

    public override void _Ready()
    {
        groundbox.BodyShapeEntered += CollideWithGround;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += velocity * (float)delta;
    }

    public void Activate()
    {
        hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
        Visible = true;
    }

    private void CollideWithGround(Rid bodyRid, Node3D body, long bodyShapeIndex, long localShapeIndex)
    {
        Visible = false;
        velocity = new Vector3();
        hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
    }
}
