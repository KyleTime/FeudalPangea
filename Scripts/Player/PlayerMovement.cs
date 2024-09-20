using Godot;
using System;

//PlayerMovement is an input/output machine type shit
//it'll rotate itself when moving, so attach this to a node just above the player model
//it will NOT move itself tho
public partial class PlayerMovement : Node3D
{
	public enum PlayerState {Grounded, OpenAir, WallSlide, Dive, Stun, StunAir, Attack, AttackAir}

	public PlayerState playerState = PlayerState.Grounded;

	public Vector3 velocity;
	
	//variables set by the Player script
	public bool grounded = false;
	public bool wall = false;
	public Basis basis; //way to orient self for velocity calculations
	public Node3D body; //model to orient towards movement

	//movement attributes
	[Export]private float speed = 10f;
	[Export]private float acceleration = 20f;
	[Export]private float deceleration = 10f;
	[Export]private float airMod = 0.5f;
	[Export]private float jumpPower = 10;
	[Export]private float stunTimer = 1;

	[Export]private float maxSlideFallingSpeed = -2.5f;

	public PlayerMovement()
	{	
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		basis = new Basis();
	}

    [Signal]
	public delegate void AttackSignalEventHandler();

	/// <summary>
	/// Reads the input from the player and changes "velocity" and updates "playerState"
	/// </summary>
	/// <param name="delta">deltatime</param>
	/// <param name="b">the basis of the camera direction</param>
	/// <param name="grounded">whether we on the ground</param>
	/// <param name="wall">whether we on the wall</param>
	public void ReadInput(double delta, Basis bas, bool grounded, bool wall)
	{
		basis = bas;
		this.grounded = grounded;
		this.wall = wall;
		ReadInput(delta);
		RotateBody();
	}

	private void ReadInput(double delta) {

		float XInput = Input.GetAxis("LEFT", "RIGHT");
		float ZInput = Input.GetAxis("FORWARD", "BACKWARD");

		Run(delta, XInput, ZInput);

		if(grounded && !(playerState == PlayerState.Grounded) && velocity.Y < 0)
			playerState = PlayerState.Grounded;
		else if(!(playerState == PlayerState.Dive) && !grounded)
			playerState = PlayerState.OpenAir;

		if(Input.IsActionJustPressed("JUMP") && playerState == PlayerState.Grounded)
		{
			playerState = PlayerState.OpenAir;
			GD.Print("JUMP!");
			Jump();
		}

		if(Input.IsActionJustPressed("DIVE") && !(playerState == PlayerState.Grounded))
		{
			playerState = PlayerState.Dive;
			GD.Print("DIVE!");
			Dive();
		}

		if(wall && !grounded && playerState == PlayerState.OpenAir)
		{
			playerState = PlayerState.WallSlide;
		}

		//gravity lol
		velocity.Y -= 9.8f * (float)delta;
		if(grounded && velocity.Y < 0)
			velocity.Y = -0.1f;
		if(playerState == PlayerState.WallSlide && velocity.Y < maxSlideFallingSpeed)
		{
			velocity.Y = maxSlideFallingSpeed;
		}

		GD.Print("STATE: " + playerState.ToString());
	}

	private void RotateBody()
	{
		Vector3 flatVelocity = new Vector3(velocity.X, 0, velocity.Z);
		if(flatVelocity.Length() > 1){
			LookAt(GlobalPosition + flatVelocity);

			Rotation = new Vector3(0, Rotation.Y, 0);
		}
	}

	private void Run(double delta, float XInput, float ZInput) {
		
		if(playerState == PlayerState.OpenAir)
		{
			delta *= airMod;
		}
		else if(playerState == PlayerState.Dive)
		{
			//slow down acceleration whilst diving
			delta *= 0.1f;
		}

		Vector3 xDir = basis.X;
		xDir.Y = 0;
		xDir = xDir.Normalized();
		Vector3 zDir = basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		if(XInput != 0 || ZInput != 0)
		{
			velocity += XInput * xDir * acceleration * (float)delta;
			velocity += ZInput * zDir * acceleration * (float)delta;

			float runningSpeed = new Vector3(velocity.X, 0, velocity.Z).Length();

			//this will need to be changed later, it caps speed to the "speed" variable
			//if we want momentum and shit, fix it
			if (runningSpeed > speed)
			{
				Vector3 runVector = new Vector3(velocity.X, 0, velocity.Z).Normalized() * speed;
				velocity = new Vector3(runVector.X, velocity.Y, runVector.Z);
			}
		}
		else if(!(playerState == PlayerState.Dive))
		{
			Vector3 runVector = velocity;
			float yVel = velocity.Y;
			velocity.Y = 0;
			velocity -= velocity.Normalized() * deceleration * (float)delta;
			velocity.Y = yVel;
			
			if(!((runVector.X * velocity.X) > 0))
			{
				velocity.X = 0;
			}

			if(!((runVector.Z * velocity.Z) > 0))
			{
				velocity.Z = 0;
			}
		}
	}

	private void Jump() {
		velocity.Y = jumpPower;
	}

	private void Dive() {

		velocity.Y = 0;

		Vector3 zDir = basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = -speed * zDir;
	}

	private void Attack() {
		//emit the attack signal
		EmitSignal(SignalName.AttackSignal);
	}
}
