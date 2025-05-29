using Godot;
using System;
using CreatureBehaviors.CreatureStates;

public partial class RagdollCreature : RigidBody3D, ICreature
{
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

    public Vector3 GetCreatureVelocity()
    {
        return LinearVelocity;
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
