using Godot;
using System;

public partial class Enemy : Node3D, Creature
{
    public void _Process()
    {
        
    }

    public void ChangeHP(int change, DamageSource source)
    {
        Death(source);
    }

    public void Death(DamageSource source)
    {
        Free();
    }

    public int GetHP()
    {
        return 1;
    }

    public CreatureState GetState()
    {
        return CreatureState.OpenAir;
    }

    public void Stun(float time)
    {
        
    }

    public void Push(Vector3 force)
    {
        throw new NotImplementedException();
    }

}
