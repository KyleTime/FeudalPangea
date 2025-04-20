using Godot;
using System;
using System.Diagnostics;

//PlayerMovement is an input/output machine type shit
//it'll rotate itself when moving, so attach this to a node just above the player model
//it will NOT move itself tho
public partial class PlayerMovement : Node3D
{
	public CreatureState creatureState = CreatureState.Grounded;
	public bool changedThisFrame = false; //keep track of whether the state was changed this frame (only one state transition per frame)

	public Vector3 velocity;
	
	//variables set by the Player script
	public bool grounded = false;
	public bool wall = false;
	public Basis basis; //way to orient self for velocity calculations
	public RayCast3D wallJumpRay; //ray that determines whether I can currently jump off a wall

	//How fast, at max, should the player move?
	[Export]public float speed = 10f;

	//How fast should the player fall? (multiplier)
	[Export]private float gravityMod = 2;

	//How quickly should the player accelerate? (higher number, higher acceleration)
	[Export]private float acceleration = 20f;

	//How quickly should the player decelerate? (higher number, higher deceleration)
	[Export]private float deceleration = 10f;

	//How much air control should the player have? (0 - 1)
	[Export]private float airMod = 0.5f;

	//How high should a jump send the player? (velocity)
	[Export]private float jumpPower = 10;

	private float stunTimer = 1;

	//How much should a dive push the player upwards? (velocity)
	[Export]private float diveUpdraft = 5;

	//How much speed should diving give? (multiplier on speed)
	[Export]private float diveSpeedMod = 2;

	//How much air control should the player have while diving? (0 - 1)
	[Export]private float diveAirMod = 0.25f;

	//How much of the player's horizontal momentum should push them back after a bonk. (0 - 1)
	[Export]private float bonkPushMod = 0.25f;

	//How high a wall jump should send the player (velocity)
	[Export]private float wallJumpMod = 1;

	//how far a wall should push the player away horizontally. (velocity)
	[Export]private float wallPushMod = 1;

	//maximum speed a player should move while sliding down a wall. (velocity)
	[Export]private float maxSlideFallingSpeed = -2.5f;

	public bool animationDone = false;

	//determines how quickly vertical velocity decays after letting go of the jump button. (0 - 1)
	[Export] public float jumpEndDecay;

	//is true if the player just started to jump, used to track when to cut off upward velocity
	private bool jumping = false;

	//is true if the player wall jumped, used to track when to cut off upward velocity
	private bool wallJumping = false;

	//spells!
	int selectedSpell = 0;
	Spell[] spells = new Spell[3];


	public PlayerMovement()
	{	
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		basis = new Basis();
		wallJumpRay = GetNode<RayCast3D>("WallJumpRay");
		spells[0] = new DoubleJumpSpell(this);
	}

    [Signal]
	public delegate void WaitForAnimationSignalEventHandler();

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

		changedThisFrame = false; //reset

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
			case CreatureState.Bonk:
				State_Bonk(delta);
				break;
			case CreatureState.Stun:
				State_Stun(delta);
				break;
			case CreatureState.StunAir:
				State_StunAir(delta);
				break;
			case CreatureState.Dead:
				State_Dead(delta);
				break;
			case CreatureState.DeadAir:
				State_DeadAir(delta);
				break;
			case CreatureState.Casting:
				spells[selectedSpell].CastState(delta);
				break;
			default:
				GD.PrintErr("UNIMPLEMENTED PLAYER STATE! KYLE FIX THIS SHIT!");
				break;
		}

		HandleHoldJump();
	}

	//Handles cutting off the player's jump if they release the jump button
	private void HandleHoldJump(){
		if(jumping && velocity.Y <= 0 || velocity.Y > jumpPower){
			jumping = false;
		}
		else if(jumping && Input.IsActionJustReleased("JUMP")){
			jumping = false;
			velocity.Y *= jumpEndDecay;
		}

		if(wallJumping && velocity.Y <= 0 || velocity.Y > jumpPower * wallJumpMod){
			wallJumping = false;
		}
		else if(wallJumping && Input.IsActionJustReleased("JUMP")){
			wallJumping = false;
			velocity.Y *= jumpEndDecay;
		}
	}

	private void Gravity(float delta){
		//gravity lol
		velocity.Y -= 9.8f * delta * gravityMod;
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
		Run(delta * mod, XInput, ZInput, decelerate);
	}

#region States

	private void SetState(CreatureState newState, bool autoActive = true)
	{
		if(changedThisFrame)
			return;

		changedThisFrame = true;

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
				// case CreatureState.Attack:
				// 	WaitForAnimation();
				// 	break;
				// case CreatureState.AttackAir:
				// 	WaitForAnimation();
				// 	break;
				// case CreatureState.AttackPoke:
				// 	WaitForAnimation();
				// 	break;
			}
		}
	}

	private void State_Grounded(double delta)
	{
		Move(delta);
		RotateBody();

		if(Input.IsActionJustPressed("JUMP"))
		{
			Jump();
		}

		// TryTransition(AttackCond(), CreatureState.Attack);
		// TryTransition(AttackPokeCond(), CreatureState.AttackPoke);
		TryTransition(OpenAirCond(), CreatureState.OpenAir);
		TryTransition(WallSlideCond(), CreatureState.WallSlide);
		TryTransition(DiveCond(), CreatureState.Dive);

		//cast!
		TryTransition(CastCond(), CreatureState.Casting);
		
		Gravity((float)delta);
	}

	private void State_OpenAir(double delta)
	{
		

		Move(delta, airMod, false);
		RotateBody();

		// TryTransition(AttackAirCond(), CreatureState.AttackAir);
		// TryTransition(AttackPokeCond(), CreatureState.AttackPoke);
		TryTransition(WallSlideCond(), CreatureState.WallSlide);
		TryTransition(GroundedCond(), CreatureState.Grounded);
		TryTransition(DiveCond(), CreatureState.Dive);

		//cast!
		TryTransition(CastCond(), CreatureState.Casting);

		Gravity((float)delta);
	}

	private void State_Dive(double delta)
	{
		//note, the mod is currently the square of airMod
		//this sucks, why
		Move(delta, airMod * diveAirMod, false);
		RotateBody();

		TryTransition(GroundedCond(), CreatureState.Grounded);

		if(BonkCond())
		{
			velocity = -velocity * bonkPushMod;
			TryTransition(true, CreatureState.Bonk);
		}

		Gravity((float)delta);
	}

	private void State_Bonk(double delta){
		
		Move(delta, 0, true);

		if(GroundedCond())
		{
			Stun(0.25f);
		}

		Gravity((float)delta);
	}

	private void State_WallSlide(double delta)
	{
		Move(delta, 0.6f);
		RotateBody();

		if(Input.IsActionJustPressed("JUMP") && wallJumpRay.GetCollider() != null)
		{
			WallJump();
			
		}

		TryTransition(GroundedCond(), CreatureState.Grounded);
		//we don't want to transition out unless we're definitely not on a wall
		if(wallJumpRay.GetCollider() == null)
			TryTransition(OpenAirCond(), CreatureState.OpenAir);

		Gravity((float)delta);
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
			TryTransition(GroundedCond(), CreatureState.Grounded);
			TryTransition(OpenAirCond(), CreatureState.OpenAir);
		}

		Gravity((float)delta);
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
			TryTransition(OpenAirCond(), CreatureState.OpenAir);
			TryTransition(GroundedCond(), CreatureState.Grounded);
		}

		Gravity((float)delta);
	}

	//attack is unique because most of the magic (for now) acts through the animator, so the state here just kinda chills out
	// private void State_Attack(double delta)
	// {
	// 	RotateBody(1);

	// 	if(animationDone)
	// 	{
	// 		TryTransition(GroundedCond(), CreatureState.Grounded);
	// 		TryTransition(OpenAirCond(), CreatureState.OpenAir);
	// 	}

	// 	Decelerate(delta);
	// 	Gravity((float)delta);
	// }

	// private void State_AttackAir(double delta)
	// {
	// 	RotateBody(1);

	// 	if(animationDone)
	// 	{
	// 		TryTransition(OpenAirCond(), CreatureState.OpenAir);
	// 		TryTransition(GroundedCond(), CreatureState.Grounded);
	// 	}

	// 	TryTransition(DiveCond(), CreatureState.Dive);
	// 	Decelerate(delta);
	// 	Gravity((float)delta);
	// }

	// private void State_AttackPoke(double delta)
	// {
	// 	RotateBody(1);

	// 	if(animationDone)
	// 	{
	// 		bool pokeCond = PokeStuckCond();
	// 		TryTransition(pokeCond, CreatureState.PokeStuck);

	// 		if(!pokeCond)
	// 		{
	// 			TryTransition(GroundedCond(), CreatureState.Grounded);
	// 			TryTransition(OpenAirCond(), CreatureState.OpenAir);
	// 		}
	// 	}
		
	// 	Decelerate(delta);
	// 	Gravity((float)delta);
	// }

	// private void State_PokeStuck(double delta){
	// 	velocity.Y = 0;

	// 	if(pokeRay.GetCollider() != null && (pokeRay.GlobalPosition - pokeRay.GetCollisionPoint()).Length() < 2)
	// 	{
	// 		velocity = new Vector3(0, 0, -10f) * Basis.Z;
	// 	}
	// 	else
	// 	{
	// 		velocity = new Vector3(0, 0, 0);
	// 	}

	// 	if(Input.IsActionJustPressed("JUMP"))
	// 	{
	// 		if(WallSlideCond())
	// 			WallJump();
	// 		else
	// 			Jump();

	// 		TryTransition(OpenAirCond(), CreatureState.OpenAir);
	// 		TryTransition(GroundedCond(), CreatureState.Grounded);
	// 		TryTransition(WallSlideCond(), CreatureState.WallSlide);
			

	// 	}
	// }

	private void State_Dead(double delta)
	{
		if(!grounded)
			SetState(CreatureState.DeadAir);

		Decelerate(delta);
		Gravity((float)delta);
	}

	private void State_DeadAir(double delta)
	{
		if(grounded)
			SetState(CreatureState.Dead);

		Decelerate(delta);
		Gravity((float)delta);
	}
#endregion

#region Transitions

	public bool TryTransition(bool condition, CreatureState state)
	{
		if(condition)
		{
			SetState(state);
		}

		return condition;
	}

	/// <summary>
	/// Checks if we can go back to grounded
	/// </summary>
	public bool GroundedCond()
	{
		return grounded && velocity.Y < 0;
	}

	/// <summary>
	/// Checks if we are in the air
	/// </summary>
	public bool OpenAirCond()
	{
		return !grounded && !wall;
	}

	public bool WallSlideCond()
	{
		return !grounded && wall;
	}

	public bool DiveCond()
	{
		return Input.IsActionJustPressed("DIVE");
	}

	public bool BonkCond()
	{
		return wall;
	}

	public bool CastCond()
	{
		if(Input.IsActionJustPressed("CAST1") && spells.Length > 0 && spells[0] != null)
		{
			selectedSpell = 0;
			return true;
		}
		if(Input.IsActionJustPressed("CAST2") && spells.Length > 1 && spells[1] != null)
		{
			selectedSpell = 1;
			return true;
		}
		if(Input.IsActionJustPressed("CAST3") && spells.Length > 2 && spells[2] != null)
		{
			selectedSpell = 2;
			return true;
		}

		return false;
	}

	// private bool AttackCond()
	// {
	// 	return Input.IsActionJustPressed("ATTACK") && grounded;
	// }

	// private bool AttackAirCond()
	// {
	// 	return Input.IsActionJustPressed("ATTACK") && !grounded;
	// }

	// private bool AttackPokeCond()
	// {
	// 	return Input.IsActionJustPressed("POKE");
	// }

	// private bool PokeStuckCond()
	// {
	// 	return pokeRay.GetCollider() != null;
	// }

#endregion

#region Movement Code

	private void RotateBody(float mod = 1)
	{
		Vector3 flatVelocity = new Vector3(velocity.X, 0, velocity.Z) * mod;
		if(flatVelocity.Length() > 1){
			LookAt(GlobalPosition - flatVelocity);

			Rotation = new Vector3(0, Rotation.Y, 0);
		}
	}

	private void Run(double delta, float XInput, float ZInput, bool decelerate = true) {

		Vector3 xDir = GetXDirection();
		Vector3 zDir = GetZDirection();

		if(XInput != 0 || ZInput != 0)
		{
			Accelerate(XInput, ZInput, xDir, zDir, delta, decelerate);
		}
		else if(decelerate)
		{
			Decelerate(delta);
		}
	}

	public void Accelerate(float XInput, float ZInput, Vector3 xDir, Vector3 zDir, double delta, bool decelerate = true)
	{
		Vector3 nextVel = velocity;

		nextVel += XInput * xDir * acceleration * (float)delta;
		nextVel += ZInput * zDir * acceleration * (float)delta;

		float runningSpeed = new Vector3(velocity.X, 0, velocity.Z).Length();
		float nextRunningSpeed = new Vector3(nextVel.X, 0, nextVel.Z).Length();

		//GD.Print("Running Speed: " + runningSpeed + " Next Speed: " + nextRunningSpeed);

		//if we go over speed cap, cap it
		if (runningSpeed < speed + 0.1f && nextRunningSpeed > speed)
		{
			Vector3 runVector = new Vector3(nextVel.X, 0, nextVel.Z).Normalized() * speed;
			velocity = new Vector3(runVector.X, velocity.Y, runVector.Z);
		}
		//if we're under the speed cap, or we're slowing down, accelerate as normal
		else if(nextRunningSpeed < speed || nextRunningSpeed <= runningSpeed)
		{
			velocity = nextVel;
		}
		//we must be trying to go faster while already going too fast, decelerate instead
		else if(decelerate)
		{
			Decelerate(delta);
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
		jumping = true;
	}

	private void WallJump(){

		Vector3 zDir = Transform.Basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = -zDir * speed * wallPushMod;
		velocity.Y = jumpPower * wallJumpMod;

		wallJumping = true;
	}

	private void Dive() {

		velocity.Y = 0;

		Vector3 zDir = Transform.Basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		velocity = speed * diveSpeedMod * zDir;
		velocity.Y = diveUpdraft;
	}
#endregion


#region Forced Transitions
	//starts a whole chain of events that causes "animationDone" to be false until the animation triggered by the current state has completed
	//see the attack states for an example
	private void WaitForAnimation() {
		EmitSignal(SignalName.WaitForAnimationSignal);
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

	#endregion

	#region Helpers

//pretty sure this doesn't work anymore but anyway
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

	private Vector3 GetZDirection(){
		Vector3 zDir = basis.Z;
		zDir.Y = 0;
		zDir = zDir.Normalized();

		return zDir;
	}

	private Vector3 GetXDirection(){
		Vector3 xDir = basis.X;
		xDir.Y = 0;
		xDir = xDir.Normalized();

		return xDir;
	}

	#endregion
}
