using Godot;
using System;

public partial class MovingPlatform : AnimatableBody3D
{
    public override void _Process(double delta)
    {
        // GlobalPosition += new Vector3(50, 0, 0) * (float)delta;
        RotateY((float)delta * 1);
    }

}
