using Godot;
using System;
using System.Threading.Tasks;

//this class is kinda like an aggregate class of all the different player components
//it tells them all what to do, but each component can work independently, pretty slick huh?
public partial class Player : CharacterBody3D, Creature
{
	public PlayerMovement move;
	public PlayerHealth health;
	public PlayerCamera cam;
	public PlayerAnimation anim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//components!
		move = GetNode<PlayerMovement>("Movement");
		health = GetNode<PlayerHealth>("Health");
		cam = GetNode<PlayerCamera>("CamOrigin");
		anim = GetNode<PlayerAnimation>("AnimationPlayer"); anim.SetMaxSpeed(move.speed);

		move.AttackSignal += Attack;
		move.StateChange += HandleStateChange;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		move.ReadInput(delta, cam.GetBasis(), IsOnFloor(), IsOnWall());

		if(move.creatureState == CreatureState.Attack || move.creatureState == CreatureState.AttackAir)
		{

		}

		if(Input.IsActionJustPressed("QUIT"))
			GetTree().Quit();
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = move.velocity;
		Vector3 flatVel = move.velocity;
		flatVel.Y = 0;
		anim.SetSpeed(flatVel.Length()); //inform the animator of what we doing
		if(move.creatureState == CreatureState.Grounded) //keep informing the animator if we on the ground lol
			anim.UpdateAnimation(move.creatureState);
		MoveAndSlide();
	}


	private void Attack()
	{

	}

	private void HandleStateChange()
	{
		anim.UpdateAnimation(move.creatureState);
	}
	
	//Creature methods
    public int GetHP()
    {
        return 10;
    }

    public void ChangeHP(int change, DamageSource source)
    {
        
    }

	public async void Death(DamageSource source)
	{
		//TODO: Add a funny little death animation
		if(source == DamageSource.Fall)
			GD.Print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		await Task.Delay(3000);
		GetTree().ReloadCurrentScene();
	}

    public void Stun(float time)
    {
        
    }

	public CreatureState GetState()
	{
		return move.creatureState;
	}
}
