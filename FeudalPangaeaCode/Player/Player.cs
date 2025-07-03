using Godot;
using System;
using System.Collections.Generic;
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
	public List<Interactable> interactables = new List<Interactable>();
	int highlightedInteractable = -1;

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
		move.AnimationOverride += AnimationOverride;
		move.StateChange += HandleStateChange;
		move.PositionChange += ChangePosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		iFrames -= delta;

		move.ReadInput(delta, cam.GetBasis(), IsOnFloor(), IsOnWall());

		//this bit of code tries to find an interactable nearby that the character is looking at
		//probably optimize and separate out later or something
		int closestIndex = -1;
		if (interactables.Count > 1)
		{
			float maxDot = -2f;
			for (int i = 0; i < interactables.Count; i++)
			{
				if (!interactables[i].InRangeOfPlayer())
				{
					interactables[i].Highlighted(false);
					interactables.RemoveAt(i);
					i--;
				}
				else
				{
					Vector2 playerLook = new Vector2(Basis.Z.X, Basis.Z.Z).Normalized();
					Vector3 interPosDir = (interactables[i].GetPosition() - GlobalPosition).Normalized();
					Vector2 interactDir = new Vector2(interPosDir.X, interPosDir.Z);

					float dot = KMath.DotProduct(playerLook, interactDir);

					if (dot > maxDot)
					{
						maxDot = dot;
						closestIndex = i;
					}
				}
			}
		}
		else if (interactables.Count == 1)
		{
			if (!interactables[0].InRangeOfPlayer())
			{
				interactables[0].Highlighted(false);
				interactables.RemoveAt(0);
				highlightedInteractable = -1;
			}
			else
			{
				interactables[0].Highlighted(true);
				highlightedInteractable = 0;
			}
		}
		else
		{
			highlightedInteractable = -1;
		}

		if (closestIndex != -1 && highlightedInteractable != closestIndex)
		{
			if(highlightedInteractable != -1)
				interactables[highlightedInteractable].Highlighted(false);
			interactables[closestIndex].Highlighted(true);
			highlightedInteractable = closestIndex;
		}

		if (Input.IsActionJustPressed("INTERACT") && highlightedInteractable != -1)
		{
			interactables[highlightedInteractable].Interact();
		}

		// if(Input.IsActionJustPressed("QUIT"))
		// 	GetTree().Quit();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(anim.IsRootMotion())
			move.ApplyRootMotion(anim.GetRootMotionPosition(), GetProcessDeltaTime());
		if(IsOnCeiling())
			move.velocity.Y = 0;

		Velocity = move.velocity;
		Vector3 flatVel = move.velocity;
		flatVel.Y = 0;
		anim.SetSpeed(flatVel.Length()); //inform the animator of what we doing
		if (move.creatureState == CreatureState.Grounded) //keep informing the animator if we on the ground lol
			anim.UpdateAnimation(move.creatureState);

		MoveAndSlide();
	}

	private void ChangePosition(Vector3 pos)
	{
		Position = pos;
	}

	private async void WaitForAnimation()
	{
		move.animationDone = false;
		while (anim.anim.IsPlaying()) { await Task.Delay(10); }
		move.animationDone = true;
	}

	private void AnimationOverride(String animName)
	{
		anim.AnimationOverride(animName);
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
		if (iFrames > 0)
		{
			return;
		}

		HP = Mathf.Clamp(HP + change, 0, MAX_HP);

		EmitSignal(SignalName.HealthChange, HP, MAX_HP);

		if (HP <= 0)
		{
			Death(source);
			return;
		}
		else
		{
			iFrames = 2f;
		}

		switch (source)
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
		if (source == DamageSource.Fall)
			GD.Print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		await hud.HUD_Death_Animation();
		LevelManager.currentLevel.ReloadLevel();
	}

	public void ResetPlayer()
	{
		HP = MAX_HP;
		GlobalPosition = new Vector3();
		move.SetState(CreatureState.Grounded);
	}

	public void Stun(float time)
	{
		move.Stun(time);
	}

	public void Push(Vector3 force)
	{
		move.Push(force);
	}

	public CreatureState GetState()
	{
		return move.creatureState;
	}

	public Vector3 GetCreaturePosition()
	{
		return GlobalPosition;
	}

	public Vector3 GetCreatureCenter()
	{
		return GlobalPosition + new Vector3(0, 1.375f, 0);
	}

	public Vector3 GetCreatureVelocity()
	{
		return move.velocity;
	}

	public bool IsProtectedUnlessStunned()
	{
		return false;
	}
}
