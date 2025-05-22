using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

//this class is kinda like an aggregate class of all the different player components
//it tells them all what to do, but each component can work independently, pretty slick huh?
public partial class Player : CharacterBody3D, ICreature
{
	public static Player player; //this breaks if we have multiple player objects

	private int HP = 60;
	private int MAX_HP = 60;

	private double iFrames = 0;

	public PlayerMovement move;
	public PlayerCamera cam;
	public PlayerAnimation anim;
	public HUDHandler hud;

	[Signal]
	public delegate void HealthChangeEventHandler(int HP, int MAX_HP);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = this;

		//components!
		move = GetNode<PlayerMovement>("Body");
		cam = GetNode<PlayerCamera>("CamOrigin");
		anim = GetNode<PlayerAnimation>("Body/Body_Center/Red_Psycho"); anim.SetMaxSpeed(move.speed);
		hud = GetNode<HUDHandler>("HUD");

		move.WaitForAnimationSignal += WaitForAnimation;
		move.StateChange += HandleStateChange;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		iFrames -= delta;

		move.ReadInput(delta, cam.GetBasis(), IsOnFloor(), IsOnWall());

		// if(Input.IsActionJustPressed("QUIT"))
		// 	GetTree().Quit();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(IsOnCeiling())
			move.velocity.Y = 0;

		Velocity = move.velocity;
		Vector3 flatVel = move.velocity;
		flatVel.Y = 0;
		anim.SetSpeed(flatVel.Length()); //inform the animator of what we doing
		if(move.creatureState == CreatureState.Grounded) //keep informing the animator if we on the ground lol
			anim.UpdateAnimation(move.creatureState);

		MoveAndSlide();
	}


	private async void WaitForAnimation()
	{
		move.animationDone = false;
		while(anim.anim.IsPlaying()) { await Task.Delay(10); }
		move.animationDone = true;
	}

	private void HandleStateChange()
	{
		anim.UpdateAnimation(move.creatureState);
	}
	
	//Creature methods
    public int GetHP()
    {
        return HP;
    }

    public void ChangeHP(int change, DamageSource source)
    {
		if(iFrames > 0)
		{
			return;
		}

		HP = Mathf.Clamp(HP + change, 0, MAX_HP);

		EmitSignal(SignalName.HealthChange, HP, MAX_HP);

		if(HP <= 0){
			Death(source);
			return;
		}
		else{
			iFrames = 2f;
		}

		switch(source)
		{
			case DamageSource.Bonk:
				Stun(1f);
				break;
			case DamageSource.Fall:
				Stun(0.5f);
				break;
			default:
				Stun(1f);
				break;
		}
    }

	private async void Death(DamageSource source)
	{
		move.Die();
		if(source == DamageSource.Fall)
			GD.Print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		await hud.HUD_Death_Animation();
		GetTree().ReloadCurrentScene();
	}

    public void Stun(float time)
    {
        move.Stun(time);
    }

	public void Push(Vector3 force)
	{
		move.velocity += force;
	}

	public CreatureState GetState()
	{
		return move.creatureState;
	}

    public Vector3 GetPosition()
    {
		return Position;
    }

    public Vector3 GetVelocity()
    {
		return Velocity;
    }

}
