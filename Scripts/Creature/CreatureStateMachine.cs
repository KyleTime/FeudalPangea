using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

public partial class CreatureStateMachine : Node3D, Creature
{
	//STATE
	private CreatureState creatureState;

	//ATTRIBUTES
	private Vector3 velocity;

	private Dictionary<CreatureState, CreatureStateModule> moduleMap;

	struct CreatureStateAnimationInfo{
		public String animation;
		public bool scaleWithPlayerSpeed;
		public float playSpeed;
	}
	private Dictionary<CreatureState, CreatureStateAnimationInfo> animationMap;

	public CreatureStateMachine(){
		//TODO: make the constructor for this work, maybe use a Factory or a Builder?
	}

    public void Run(double delta, Basis facing, bool grounded, bool wall, AnimationPlayer player)
	{
		CreatureStateModule module = moduleMap[creatureState];

		Animate(player);

		module.RunState(this, delta, facing, grounded, wall); //runs the logic inside the module for whatever we're doing

		HandleStateTransition(module, delta, facing, grounded, wall, player.IsPlaying()); //checks if we can enter another state
	}

    private void Animate(AnimationPlayer player)
	{
		CreatureStateAnimationInfo animInfo = animationMap[creatureState];

		float speed;
		
		if(animInfo.scaleWithPlayerSpeed)
			speed = velocity.Length();
		else
			speed = animInfo.playSpeed;
		
		player.Play(animInfo.animation, -1, speed);
	}

	private bool HandleStateTransition(CreatureStateModule module, double delta, Basis facing, bool grounded, bool wall, bool animationDone)
	{
		foreach(var pair in moduleMap){
			//if we're allowed to transition, and we can transition, then transition
			if(module.CheckTransitionToNewState(pair.Key, animationDone) && pair.Value.TransitionCondition(this, delta, facing, grounded, wall))
			{
				creatureState = pair.Value.GetCreatureState();
				pair.Value.StartState(this, delta, facing, grounded, wall);
				return true;
			}
		}

		return false;
	}

	#region CREATURE IMPLEMENTATION
    public void ChangeHP(int change, DamageSource source)
    {
        throw new NotImplementedException();
    }

    public int GetHP()
    {
        throw new NotImplementedException();
    }

    public CreatureState GetState()
    {
        throw new NotImplementedException();
    }

    public void Stun(float time)
    {
        throw new NotImplementedException();
    }

    public void Push(Vector3 force)
    {
        throw new NotImplementedException();
    }
	#endregion
}
