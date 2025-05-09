using Godot;
using System;
using System.Threading.Tasks;

public partial class test_moving_platform : RigidBody3D
{
    public async override void _Ready()
	{
        await Task.Delay(3000);
        LinearVelocity = new Vector3(0, 0, 5);
    }

    public override void _Process(double delta)
	{
        
    }
}
