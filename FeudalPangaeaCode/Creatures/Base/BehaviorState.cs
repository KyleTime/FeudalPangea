using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Interface to be used by the CreatureStateMachine class for state behavior.
/// </summary>
public abstract class BehaviorState {
    
    public string name = "name";
    protected ICreature target;
    protected readonly CreatureState creatureState;

    protected Dictionary<BehaviorCondition, BehaviorState> transitions = new Dictionary<BehaviorCondition, BehaviorState>();

    public BehaviorState(CreatureState creatureState){
        this.creatureState = creatureState;
    }

    /// <summary>
    /// Add a transition to the dictionary.
    /// </summary>
    /// <param name="cond">Condition to check for.</param>
    /// <param name="state">Transition to swap to.</param>
    public void AddTransition(BehaviorCondition cond, BehaviorState state){
        transitions.Add(cond, state);
    }

    /// <summary>
    /// Gets the state that this BehaviorState implementation represents.
    /// </summary>
    /// <returns>Corresponding CreatureState</returns>
    public CreatureState GetCreatureState(){
        return creatureState;
    }

    /// <summary>
    /// Runs the state logic and returns the new velocity after modifications.
    /// </summary>
    /// <param name="self">The CreatureStateMachine this state is acting on</param>
    /// <returns>The new velocity.</returns>
    public abstract Vector3 GetStepVelocity(CreatureStateMachine self);

    public void SetTarget(ICreature target){
        this.target = target;
    }

    /// <summary>
    /// Checks the condition for each state transition until it finds a valid one. Then, it attempts to extract a target and assign it to the new state.
    /// </summary>
    /// <returns>The new state to use</returns>
    public BehaviorState StateTransition(){
        
        foreach(var pair in transitions){
            if(pair.Key.Condition()){

                ICreature trgt = pair.Key.GetTarget();
                if(trgt != null){
                    pair.Value.SetTarget(trgt);
                }

                return pair.Value;
            }
        }

        return null;
    }
}