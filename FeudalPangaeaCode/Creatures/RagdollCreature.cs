using Godot;
using System;
using CreatureBehaviors.CreatureStates;

public partial class RagdollCreature : RigidBody3D, ICreature
{
    [Export] public CollisionShape3D collider;
    [Export] public Area3D hurtbox;

    public int GetHP()
    {
        return 0;
    }

    public void ChangeHP(int change, DamageSource source)
    {
        return;
    }

    public Vector3 GetCreaturePosition()
    {
        return GlobalPosition;
    }

    public Vector3 GetCreatureCenter()
    {
        return GlobalPosition + new Vector3(0, 1, 0);
    }

    public Vector3 GetCreatureVelocity()
    {
        return LinearVelocity;
    }

    public bool IsProtectedUnlessStunned()
    {
        return false;
    }

    public void Stun(float time)
    {
        return;
    }

    public void Push(Vector3 force)
    {
        LinearVelocity += force;
        ApplyTorque(force); 
    }

    public CreatureState GetState()
    {
        return CreatureState.Dead;
    }
}
