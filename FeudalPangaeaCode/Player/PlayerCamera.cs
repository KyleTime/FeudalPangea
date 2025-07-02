using Godot;
using System;
using System.Threading.Tasks;

//this script is to be placed on the "CamOrigin" node
public partial class PlayerCamera : Node3D
{
	[Export] public float sens = 0.3f;

	private Node3D camOriginX;

	private Camera3D camera;
	[Export] Camera3D cinematicCamera; //used for things like cutscenes and death

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camOriginX = GetNode<Node3D>("CamOriginX");
		camera = GetNode<Camera3D>("CamOriginX/SpringArm3D/Camera3D");

		await Task.Delay(10);

		RemoveChild(cinematicCamera);
		GetTree().Root.GetChild(0).AddChild(cinematicCamera);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public new Basis GetBasis()
	{
		return camera.GlobalBasis;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion m = (InputEventMouseMotion)@event;

			RotateY((float)(Math.PI / 180 * -m.Relative.X * sens));
			camOriginX.RotateX((float)(Math.PI / 180 * -m.Relative.Y * sens));

			camOriginX.Rotation = new Vector3(Math.Clamp(camOriginX.Rotation.X, (float)(Math.PI / 180 * -90), (float)(Math.PI / 180 * 45)), camOriginX.Rotation.Y, camOriginX.Rotation.Z);
		}
    }

	public void DeathCam(int HP, int MAX_HP)
	{
		if (HP <= 0)
		{
			camera.Current = false;
			cinematicCamera.Current = true;

			cinematicCamera.GlobalPosition = camera.GlobalPosition;
			cinematicCamera.GlobalBasis = camera.GlobalBasis;
		}
		else
		{
			camera.Current = true;
			cinematicCamera.Current = false;
		}
	}
}
