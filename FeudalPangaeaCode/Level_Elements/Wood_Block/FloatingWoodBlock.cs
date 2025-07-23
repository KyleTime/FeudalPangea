using Godot;
using System;
using System.Collections;

public partial class FloatingWoodBlock : RigidBody3D, ICreature
{
    private CollisionShape3D shape;
    private MeshInstance3D mesh;

    public bool IsMajor => false;

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

    public int GetHP()
    {
        return 1;
    }
    public void ChangeHP(int change, DamageSource source)
    {
        if (change < 0 && source == DamageSource.Fire)
        {
            QueueFree();
        }
    }

    /// <summary>
    /// Returns the position at the feet of the creature.
    /// </summary>
    public Vector3 GetCreaturePosition()
    {
        return GlobalPosition;
    }

    /// <summary>
    /// Returns the position at the center of the creature.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCreatureCenter()
    {
        return GlobalPosition;
    }
    public Vector3 GetCreatureVelocity()
    {
        return new Vector3();
    }

    public void Stun(float time)
    {
        return;
    }
    public void Push(Vector3 force)
    {
        LinearVelocity += force;
    }

    public CreatureState GetState()
    {
        return CreatureState.Grounded;
    }
}
