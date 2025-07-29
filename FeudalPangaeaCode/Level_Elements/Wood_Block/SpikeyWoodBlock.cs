using Godot;
using System;
using System.Collections;

public partial class SpikeyWoodBlock : Node3D, ICreature
{
    public bool IsMajor => false;

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
    }

    public CreatureState GetState()
    {
        return CreatureState.Grounded;
    }
}
