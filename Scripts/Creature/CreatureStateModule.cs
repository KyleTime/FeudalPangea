using Godot;
using System;
using System.Collections.Generic;

public abstract partial class CreatureStateModule
{
    private CreatureState moduleState;
    private Dictionary<CreatureState, bool> ableToTransitionTo;

    //runs on the frame of the transition
    public abstract void StartState(CreatureStateMachine CSM, double delta, Basis facing, bool grounded, bool wall);

    //runs every frame while CSM creatureState == moduleState
    public abstract void RunState(CreatureStateMachine CSM, double delta, Basis facing, bool grounded, bool wall);

    //returns true if a Creature with the given conditions could enter this state
    public abstract bool TransitionCondition(CreatureStateMachine CSM, double delta, Basis facing, bool grounded, bool wall);

    //returns true if a Creature is allowed to go from moduleState to newState
    //also takes in "animationDone" if an override wishes to take that into account
    public virtual bool CheckTransitionToNewState(CreatureState newState, bool animationDone){
        return ableToTransitionTo[newState];
    }

    public CreatureState GetCreatureState(){
        return moduleState;
    }
}