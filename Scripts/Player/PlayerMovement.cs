using Godot;
using System;

//PlayerMovement is an input/output machine type shit
//it'll rotate itself when moving, so attach this to a node just above the player model
//it will NOT move itself tho
public partial class PlayerMovement : Node3D
{
	public CreatureState creatureState = CreatureState.Grounded;

	public Vector3 velocity;
	
	//variables set by the Player script
	public bool grounded = false;
	public bool wall = false;
	public Basis basis; //way to orient self for velocity calculations
	public RayCast3D wallJumpRay; //ray that determines whether I can currently jump off a wall
	public Area3D hitbox; //hitbox of the weapon the player is carrying NOTE: might redo how this works later

	//movement attributes
	[Export]public float speed = 10f;
	[Export]private float acceleration = 20f;
	[Export]private float deceleration = 10f;
	[Export]private float airMod = 0.5f;
	[Export]private float jumpPower = 10;
	[Export]private float stunTimer = 1;
	[Export]private float diveUpdraft = 5;
	[Export]private float diveSpeedMod = 2;
	[Export]private float wallJumpMod = 1;

	[Export]private float maxSlideFallingSpeed = -2.5f;

	[Export]private float attackTime = 1;
	private float attackTimer;

	public PlayerMovement()
	{	
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		basis = new Basis();
		wallJumpRay = GetNode<RayCast3D>("WallJumpRay");
		hitbox = GetNode<Area3D>("Weapon");
	}

    [Signal]
	public delegate void AttackSignalEventHandler();

	[Signal]
	public delegate void StateChangeEventHandler();

	/// <summary>
	/// Reads the input from the player and changes "velocity" and updates "creatureState"
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

		if(!Attacking())
		{
			RotateBody();
			hitbox.Monitorable = false;
			hitbox.Monitoring = false;
		}
		else if(hitbox.Monitoring == false)
		{
			hitbox.Monitorable = true;
			hitbox.Monitoring = true;
		}
	}

	private void ReadInput(double delta) {

		if(attackTimer < 0)
		{
			creatureState = CreatureState.OpenAir;
			attackTimer = 0;
		}

		float XInput = 0;
		float ZInput = 0;

		if(!Attacking())
		{
			XInput = Input.GetAxis("LEFT", "RIGHT");
			ZInput = Input.GetAxis("FORWARD", "BACKWARD");

			if(grounded && !(creatureState == CreatureState.Grounded) && velocity.Y < 0)
				SetState(CreatureState.Grounded);
			else if(!(creatureState == CreatureState.Dive) && !grounded)
				SetState(CreatureState.OpenAir);
		}
		else
			attackTimer -= (float)delta;
		
		Run(delta, XInput, ZInput);

		if(wall && !grounded && creatureState == CreatureState.OpenAir)
		{
			SetState(CreatureState.WallSlide);
		}

		if(Input.IsActionJustPressed("JUMP"))
		{
			if(creatureState == CreatureState.Grounded){
				SetState(CreatureState.OpenAir);
				Jump();
			}
			else if(creatureState == CreatureState.WallSlide && wallJumpRay.GetCollider() != null)
			{
				SetState(CreatureState.OpenAir);
				WallJump();
			}
		}

		if(Input.IsActionJustPressed("DIVE") && creatureState == CreatureState.OpenAir)
		{
			SetState(CreatureState.Dive);
			Dive();
		}

		if(Input.IsActionJustPressed("ATTACK") && (creatureState == CreatureState.Grounded || creatureState == CreatureState.OpenAir))
		{
			if(creatureState == CreatureState.Grounded)
				SetState(CreatureState.Attack);
			else
				SetState(CreatureState.AttackAir);
			
			Attack();
		}

		//gravity lol
		velocity.Y -= 9.8f * (float)delta;
		if(grounded && velocity.Y < 0)
			velocity.Y = -0.1f;
		if(creatureState == CreatureState.WallSlide && velocity.Y < maxSlideFallingSpeed)
		{
			velocity.Y = maxSlideFallingSpeed;
		}
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
		
		if(creatureState == CreatureState.OpenAir)
		{
			delta *= airMod;
		}
		else if(creatureState == CreatureState.Dive)
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
		else if(!(creatureState == CreatureState.Dive))
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

	private void WallJump(){

		Vector3 zDir = Transform.Basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = zDir * speed;
		velocity.Y = jumpPower * wallJumpMod;
	}

	private void Dive() {

		velocity.Y = 0;

		Vector3 zDir = Transform.Basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = -speed * diveSpeedMod * zDir;
		velocity.Y = diveUpdraft;
	}

	private void SetState(CreatureState newState)
	{
		creatureState = newState;

		EmitSignal(SignalName.StateChange);
	}

	private void Attack() {
		attackTimer = attackTime;

		LookAtInput();

		//emit the attack signal
		EmitSignal(SignalName.AttackSignal);
	}

	private void LookAtInput()
	{
		float XInput = Input.GetAxis("LEFT", "RIGHT");
		float ZInput = Input.GetAxis("FORWARD", "BACKWARD");

		if(XInput == 0 && ZInput == 0)
		{
			return;
		}

		LookAt(GlobalPosition + basis.Z * ZInput + basis.X * XInput);

		Rotation = new Vector3(0, Rotation.Y, 0);
	}

	private bool Attacking()
	{
		return creatureState == CreatureState.Attack || creatureState == CreatureState.AttackAir;
	}


}
