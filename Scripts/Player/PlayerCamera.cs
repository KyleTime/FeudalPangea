using Godot;
using System;

//this script is to be placed on the "CamOrigin" node
public partial class PlayerCamera : Node3D
{
	[Export] public float sens = 0.5f;

	private Node3D camOriginX;

	private Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camOriginX = GetNode<Node3D>("CamOriginX");
		camera = GetNode<Camera3D>("CamOriginX/SpringArm3D/Camera3D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion)
		{
			InputEventMouseMotion m = (InputEventMouseMotion) @event;

			RotateY((float)(Math.PI / 180 * -m.Relative.X * sens));
			camOriginX.RotateX((float)(Math.PI / 180 * -m.Relative.Y * sens));
			
			camOriginX.Rotation = new Vector3(Math.Clamp(camOriginX.Rotation.X, (float)(Math.PI / 180 * -90), (float)(Math.PI / 180 * 45)), camOriginX.Rotation.Y, camOriginX.Rotation.Z);
		}
    }

	public Basis GetBasis()
	{
		return camera.GlobalBasis;
	}
}
