using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

//USAGE: Create an Instance in your favorite class and then call "Run"
//then call "GetVelocity()" to get your new velocity!
//the animation will happen automatically as long as it's set up (map each state to a CreatureStateAnimationInfo)
public partial class CreatureStateMachine : Node3D, Creature
{
	//STATE
	private CreatureState initialState;
	private CreatureState creatureState;

	//ATTRIBUTES
	private Vector3 velocity;

	private Dictionary<CreatureState, CreatureStateModule> moduleMap;

	public struct CreatureStateAnimationInfo{
		public String animation;
		public float playSpeed;
		public bool scaleWithPlayerSpeed;

		public CreatureStateAnimationInfo(String animation, float playSpeed = 1, bool scaleWithPlayerSpeed = false){
			this.animation = animation;
			this.playSpeed = playSpeed;
			this.scaleWithPlayerSpeed = scaleWithPlayerSpeed;
		}
	}
	private Dictionary<CreatureState, CreatureStateAnimationInfo> animationMap;

	public CreatureStateMachine(Builder builder){
		initialState = builder.initialState;
		creatureState = builder.initialState;
		moduleMap = builder.moduleMap;
		animationMap = builder.animationMap;
	}

	public class Builder{
		public CreatureState initialState;
		public Dictionary<CreatureState, CreatureStateModule> moduleMap;
		public Dictionary<CreatureState, CreatureStateAnimationInfo> animationMap;

		public Builder InitialState(CreatureState initialState){
			this.initialState = initialState;

			return this;
		}

		public Builder ModuleMap(CreatureState state, CreatureStateModule module){
			this.moduleMap.Add(state, module);

			return this;
		}

		public Builder AnimationMap(CreatureState state, CreatureStateAnimationInfo info){
			this.animationMap.Add(state, info);

			return this;
		}

		public CreatureStateMachine Build(){
			return new CreatureStateMachine(this);
		}
	}

    public void Run(double delta, Basis facing, bool grounded, bool wall, AnimationPlayer player)
	{
		if(!moduleMap.ContainsKey(creatureState)){
			
			SwapState(initialState, delta, facing, grounded, wall);
			return;
		}

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
				SwapState(pair, delta, facing, grounded, wall);
				return true;
			}
		}

		return false;
	}

	//swaps the state to new CreatureState
	private void SwapState(CreatureState newState, double delta, Basis facing, bool grounded, bool wall){
		if(!moduleMap.ContainsKey(newState)){
			throw new Exception("STATE DOES NOT EXIST!");
		}

		moduleMap[creatureState].EndState(this, delta, facing, grounded, wall);
		creatureState = newState;
		moduleMap[newState].StartState(this, delta, facing, grounded, wall);
	}

	//swaps the state to new CreatureState without calling the EndState method of the previous state
	//this mostly exists so that if we ever accidentally enter an invalid state, the creature can recover
	private void ForceSwapState(CreatureState newState, double delta, Basis facing, bool grounded, bool wall){
		creatureState = newState;
		moduleMap[newState].StartState(this, delta, facing, grounded, wall);
	}

	//overflow mostly for convenience
	private void SwapState(KeyValuePair<CreatureState, CreatureStateModule> pair, double delta, Basis facing, bool grounded, bool wall){
		moduleMap[creatureState].EndState(this, delta, facing, grounded, wall);
		creatureState = pair.Key;
		pair.Value.StartState(this, delta, facing, grounded, wall);
	}

	#region Getters

    public int GetHP()
    {
        throw new NotImplementedException();
    }

    public CreatureState GetState()
    {
        return creatureState;
    }

	public Vector3 GetVelocity()
	{
		return velocity;
	}

	#endregion

	#region Transformations
    public void ChangeHP(int change, DamageSource source)
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
