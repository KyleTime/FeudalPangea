using Godot;
using System;

public partial class FloatingWoodBlock : RigidBody3D
{
    private CollisionShape3D shape;
    private MeshInstance3D mesh;

    /// <summary>
    /// Needed because scaling a rigidbody directly doesn't work
    /// Will resize the shape and mesh at the start of the scene
    /// </summary>
    [Export] Vector3 size = new Vector3(3, 0.5f, 3);

    private Hurtbox hurtbox;

    public override void _Ready()
    {
        shape = GetNode<CollisionShape3D>("CollisionShape3D");
        mesh = GetNode<MeshInstance3D>("MeshInstance3D");
        hurtbox = GetNode<Hurtbox>("Hurtbox");

        shape.Scale = size;
        mesh.Scale = size;
        hurtbox.collider.Scale = size;
    }

}
