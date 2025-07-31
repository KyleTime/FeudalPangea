using Godot;
using System;
using System.Collections;
using System.IO.Pipes;
using System.Threading.Tasks;

public partial class FloatingWoodBlock : Node3D, ICreature
{
    public bool IsMajor => false;

    private Hurtbox hurtbox;
    
    static int bobVariation = 0;

    static float height = 0.1f;
    static float totalTime = 5;

    Node3D position;

    public override void _Ready()
    {
        bobVariation += 1;
        bobVariation = bobVariation % 3;

        position = GetNode<Node3D>("position");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(position))
        {
            return;
        }

        float time = Time.GetTicksMsec() / 1000.0f;
        position.Position = new Vector3() with { Y = Mathf.Sin(time * 2 * Mathf.Pi / totalTime) * height };
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
        //LinearVelocity += force;
    }

    public CreatureState GetState()
    {
        return CreatureState.Grounded;
    }
}
