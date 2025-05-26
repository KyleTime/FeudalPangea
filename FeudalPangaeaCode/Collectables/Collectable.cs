using Godot;
using System;

public abstract partial class Collectable : Node3D
{
    protected double radius = 1;

    public override void _PhysicsProcess(double delta)
    {
        double dist = Math.Sqrt(Math.Pow(Position.X - Player.player.Position.X, 2) + Math.Pow(Position.Y - Player.player.move.Position.Y, 2) + Math.Pow(Position.Z - Player.player.Position.Z, 2));

        //assume that scale is always a cube, use scale
        if(dist < radius * Scale.X)
        {
            Collect();
        }
    }

    public abstract void Collect();
}
