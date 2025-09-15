using Godot;
using PhantomCamera;
using System;
using System.Threading.Tasks;

public partial class PlayerCamera : Node3D
{
	[Export] public float sens = 0.3f;
	[Export] public float minPitch = -80f;
	[Export] public float maxPitch = 70;

	public PhantomCamera3D playerCam;
	public PhantomCamera3D deathCam;
	public Camera3D cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public new Basis GetBasis()
	{
		return cam.Basis;
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.MouseMode == Input.MouseModeEnum.Captured && Input.IsActionJustPressed("CAST3"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else if (Input.IsActionJustPressed("CAST3"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}

		if(Input.MouseMode == Input.MouseModeEnum.Captured)
			SetPlayerCamRotation(playerCam, @event);
    }

	void SetPlayerCamRotation(PhantomCamera3D cam, InputEvent @event)
	{
		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion m = (InputEventMouseMotion)@event;

			Vector3 playerCamRotationDegrees = new Vector3();

			playerCamRotationDegrees = cam.GetThirdPersonRotationDegrees();

			playerCamRotationDegrees.X -= m.Relative.Y * sens;

			playerCamRotationDegrees.X = Mathf.Clamp(playerCamRotationDegrees.X, minPitch, maxPitch);

			playerCamRotationDegrees.Y -= m.Relative.X * sens;

			playerCamRotationDegrees.Y = (float)Mathf.Wrap((double)playerCamRotationDegrees.Y, 0, 360);

			cam.SetThirdPersonDegrees(playerCamRotationDegrees);
		}
	}

	public void DeathCam(int HP, int MAX_HP)
	{
		if (HP <= 0)
		{
			deathCam.Node3D.GlobalPosition = playerCam.Node3D.GlobalPosition;
			deathCam.Node3D.GlobalBasis = playerCam.Node3D.GlobalBasis;

			deathCam.Priority = 30;
			playerCam.Priority = 0;
		}
		else
		{
			deathCam.Priority = 0;
			playerCam.Priority = 30;
		}
	}
}
