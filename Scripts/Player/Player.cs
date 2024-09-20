using Godot;
using System;

//this class contains all the components of the Player
public partial class Player : CharacterBody3D, Creature
{
	public PlayerMovement move;
	public PlayerHealth health;
	public PlayerAttack attack;
	public PlayerCamera cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//components!
		move = GetNode<PlayerMovement>("Movement");
		health = GetNode<PlayerHealth>("Health");
		attack = GetNode<PlayerAttack>("Attack");
		cam = GetNode<PlayerCamera>("CamOrigin");

		move.AttackSignal += Attack;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		move.ReadInput(delta, cam.GetBasis(), IsOnFloor(), IsOnWall());

		if(Input.IsActionJustPressed("QUIT"))
			GetTree().Quit();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Basis b = cam.GetBasis();
		// Vector3 xDir = b.X;
		// xDir.Y = 0;
		// xDir = xDir.Normalized();
		// Vector3 zDir = b.Z;
		// zDir.Y = 0;
		// zDir = zDir.Normalized();
		// Velocity = new Vector3(0, move.velocity.Y, 0) + move.velocity.X * xDir + move.velocity.Z * zDir;
		Velocity = move.velocity;

		// Vector3 flatVelocity = new Vector3(Velocity.X, 0, Velocity.Z);
		// if(flatVelocity.Length() > 1){
		// 	body.LookAt(Position + flatVelocity);

		// 	body.Rotation = new Vector3(0, body.Rotation.Y, 0);
		// }

		MoveAndSlide();
	}


	private void Attack()
	{
		attack.Attack();
	}
	
	//Creature methods
    public int GetHP()
    {
        return 10;
    }

    public void ChangeHP(int change)
    {
        
    }

    public void Stun(float time)
    {
        
    }
}
