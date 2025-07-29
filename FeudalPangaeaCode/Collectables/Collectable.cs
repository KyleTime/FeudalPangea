using Godot;
using System;

public abstract partial class Collectable : Node3D
{
    protected double radius = 1;

    public override void _PhysicsProcess(double delta)
    {
        double dist = KMath.Dist3D(GlobalPosition, Player.player.GlobalPosition);

        //assume that scale is always a cube, use scale
        if(dist < radius * Scale.X)
        {
            Collect();
        }
    }

    public abstract void Collect();
}
