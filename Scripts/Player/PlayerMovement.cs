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
	[Export]private float wallPushMod = 1;

	[Export]private float maxSlideFallingSpeed = -2.5f;

	[Export]public bool doneAttacking = false;

	public PlayerMovement()
	{	
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		basis = new Basis();
		wallJumpRay = GetNode<RayCast3D>("WallJumpRay");
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
	}

	private void ReadInput(double delta) {
		switch (creatureState)
		{
			case CreatureState.Grounded:
				State_Grounded(delta);
				break;
			case CreatureState.OpenAir:
				State_OpenAir(delta);
				break;
			case CreatureState.WallSlide:
				State_WallSlide(delta);
				break;
			case CreatureState.Dive:
				State_Dive(delta);
				break;
			case CreatureState.Stun:
				State_Stun(delta);
				break;
			case CreatureState.StunAir:
				State_StunAir(delta);
				break;
			case CreatureState.Attack:
				State_Attack(delta);
				break;
			case CreatureState.AttackAir:
				State_AttackAir(delta);
				break;
			case CreatureState.Dead:
				State_Dead(delta);
				break;
			case CreatureState.DeadAir:
				State_DeadAir(delta);
				break;
			default:
				GD.PrintErr("UNIMPLEMENTED PLAYER STATE! KYLE FIX THIS SHIT!");
				break;
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

	private void Move(double delta, float mod = 1, bool decelerate = true){
		float XInput = Input.GetAxis("LEFT", "RIGHT");
		float ZInput = Input.GetAxis("FORWARD", "BACKWARD");	
		Run(delta * mod, XInput, ZInput);
	}

#region States
	private void State_Grounded(double delta)
	{
		Move(delta);
		RotateBody();

		if(Input.IsActionJustPressed("JUMP"))
		{
			Jump();
		}

		TryAttack();
		TryOpenAir();
		TryDive();
	}

	private void State_OpenAir(double delta)
	{
		Move(delta, airMod);
		RotateBody();

		TryAttackAir();
		TryWallSlide();
		TryGrounded();
		TryDive();
	}

	private void State_Dive(double delta)
	{
		//note, the mod is currently the square of airMod
		//this sucks, why
		Move(delta, airMod * airMod, false);
		RotateBody();

		TryGrounded();
	}

	private void State_WallSlide(double delta)
	{
		Move(delta, 0.6f);
		RotateBody();

		if(Input.IsActionJustPressed("JUMP") && wallJumpRay.GetCollider() != null)
		{
			WallJump();
			
		}

		TryGrounded();
		//we don't want to transition out unless we're definitely not on a wall
		if(wallJumpRay.GetCollider() == null)
			TryOpenAir();
	}

	private void State_Stun(double delta)
	{
		RotateBody(-1);

		if(stunTimer > 0){
			stunTimer -= (float)delta;

			Decelerate(delta);

			if(!grounded)
				SetState(CreatureState.StunAir);
		}
		else
		{
			TryGrounded();
			TryOpenAir();
		}
	}

	//little note: StunAir may look exactly the same as Stun, but that's largely just because the state is mostly for the animator
	private void State_StunAir(double delta)
	{
		RotateBody(-1);

		if(stunTimer > 0){
			stunTimer -= (float)delta;

			Decelerate(delta);

			if(grounded)
				SetState(CreatureState.Stun);
		}
		else
		{
			TryOpenAir();
			TryGrounded();
		}
	}

	//attack is unique because most of the magic (for now) acts through the animator, so the state here just kinda chills out
	private void State_Attack(double delta)
	{
		if(doneAttacking)
		{
			TryGrounded();
			TryOpenAir();
		}

		Decelerate(delta);
	}

	private void State_AttackAir(double delta)
	{
		RotateBody(1);

		if(doneAttacking)
		{
			TryOpenAir();
			TryGrounded();
		}

		TryDive();
		Decelerate(delta);
	}

	private void State_Dead(double delta)
	{
		if(!grounded)
			SetState(CreatureState.DeadAir);

		Decelerate(delta);
	}

	private void State_DeadAir(double delta)
	{
		if(grounded)
			SetState(CreatureState.Dead);

		Decelerate(delta);
	}
#endregion

#region Transitions
	/// <summary>
	/// Checks if we can go back to grounded
	/// </summary>
	private bool TryGrounded()
	{
		bool valid = grounded && velocity.Y < 0;

		if(valid)
			SetState(CreatureState.Grounded);

		return valid;
	}

	/// <summary>
	/// Checks if we are in the air
	/// </summary>
	private bool TryOpenAir()
	{
		bool valid = !grounded && !wall;

		if(valid)
			SetState(CreatureState.OpenAir);

		return valid;
	}

	private bool TryWallSlide()
	{
		bool valid = !grounded && wall;

		if(valid)
			SetState(CreatureState.WallSlide);

		return valid;
	}

	private bool TryDive()
	{
		bool valid = Input.IsActionJustPressed("DIVE");

		if(valid)
		{
			SetState(CreatureState.Dive);
		}

		return valid;
	}

	private bool TryAttack()
	{
		bool valid = Input.IsActionJustPressed("ATTACK") && grounded;

		if(valid)
		{
			SetState(CreatureState.Attack);
		}

		return valid;
	}

	private bool TryAttackAir()
	{
		bool valid = Input.IsActionJustPressed("ATTACK") && !grounded;

		if(valid)
		{
			velocity = basis.Z * -10;
			velocity.Y = jumpPower/2;
			SetState(CreatureState.AttackAir);
		}

		return valid;
	}
#endregion

	private void RotateBody(float mod = 1)
	{
		Vector3 flatVelocity = new Vector3(velocity.X, 0, velocity.Z) * mod;
		if(flatVelocity.Length() > 1){
			LookAt(GlobalPosition + flatVelocity);

			Rotation = new Vector3(0, Rotation.Y, 0);
		}
	}

	private void Run(double delta, float XInput, float ZInput, bool decelerate = true) {

		Vector3 xDir = basis.X;
		xDir.Y = 0;
		xDir = xDir.Normalized();
		Vector3 zDir = basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		if(XInput != 0 || ZInput != 0)
		{
			Accelerate(XInput, ZInput, xDir, zDir, delta);
		}
		else if(decelerate)
		{
			Decelerate(delta);
		}
	}

	public void Accelerate(float XInput, float ZInput, Vector3 xDir, Vector3 zDir, double delta)
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

	public void Decelerate(double delta)
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

	private void Jump() {
		velocity.Y = jumpPower;
	}

	private void WallJump(){

		Vector3 zDir = Transform.Basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = zDir * speed * wallPushMod;
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

	private void SetState(CreatureState newState, bool autoActive = true)
	{
		creatureState = newState;
		GD.Print("STATE: " + creatureState.ToString());

		EmitSignal(SignalName.StateChange);

		if(autoActive)
		{
			switch(creatureState)
			{
				case CreatureState.Dive:
					Dive();
					break;
				case CreatureState.Attack:
					Attack();
					break;
			}
		}
	}

	private void Attack() {
		doneAttacking = false;

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

	public void Stun(float time)
	{
		if(creatureState == CreatureState.Dead || creatureState == CreatureState.DeadAir)
			return; //you can't really be stunned if you dead lol

		stunTimer = time;
		SetState(CreatureState.Stun);
	}

	public void Die()
	{
		SetState(CreatureState.Dead);
	}
}
