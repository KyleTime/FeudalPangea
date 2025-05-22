using Godot;
using System;
using System.Collections.Generic;


public partial class CreatureStateMachine : CharacterBody3D, ICreature
{
    private BehaviorState state;

    //the creature that this creature is targetting (for whatever reason or purpose)
    public ICreature target;

    private int HP = 1;

    public class Builder
    {

        int HP = 1;
        BehaviorState initialState;
        Dictionary<string, BehaviorState> states = new Dictionary<string, BehaviorState>();

        public CreatureStateMachine build()
        {

            //something that's kinda funny is that we don't actually *need* to give 
            //CreatureStateMachine the whole dictionary. All of the states are already linked together!
            //This also neatly culls any unused states from memory, which is nice.

            return new CreatureStateMachine(initialState, HP);
        }

        /// <summary>
        /// Set the HP of the creature. Pretty self explanatory.
        /// </summary>
        /// <param name="hp">Amount of HP</param>
        /// <returns>The Builder! Again!</returns>
        public Builder SetHP(int hp)
        {
            HP = hp;
            return this;
        }

        /// <summary>
        /// Add a state to the dictionary in the builder. If there is no initial state, set it to this one.
        /// </summary>
        /// <param name="stateName">The name of the state for later reference.</param>
        /// <param name="state">The BehaviorState to add as a state.</param>
        /// <returns>The Builder (the builder pattern!)</returns>
        public Builder AddState(string stateName, BehaviorState state)
        {

            if (initialState == null)
            {
                initialState = state;
            }

            state.name = stateName;
            states.Add(stateName, state);
            return this;
        }

        /// <summary>
        /// Sets the initial state of the CreatureStateMachine.
        /// </summary>
        /// <param name="stateName">Name of the state to reference.</param>
        /// <returns>The Builder (it's just a builder pattern)</returns>
        public Builder SetInitialState(string stateName)
        {
            initialState = states[stateName];
            return this;
        }

        /// <summary>
        /// Creates a transition between two states based on a BehaviorCondition.
        /// </summary>
        /// <param name="current">The name of the state that the transition will start from.</param>
        /// <param name="cond">The condition by which to decide whether to transition.</param>
        /// <param name="next">The name of the state to transition into.</param>
        /// <returns>The Builder (wow! the builder pattern!)</returns>
        /// <exception cref="Exception"></exception>
        public Builder AddTransition(string current, BehaviorCondition cond, string next)
        {

            if (!states.ContainsKey(current) || !states.ContainsKey(next))
            {
                throw new Exception("One or more states request do not exist! Transition failed to be created in CreatureStateMachine!");
            }

            if (cond == null)
            {
                throw new Exception("Condition was null. Please provide a valid BehaviorCondition for the creation of the transition in CreatureStateMachine!");
            }

            //get the BehaviorState of the referenced states
            BehaviorState curState = states[current];
            BehaviorState nextState = states[next];

            //add the transition
            curState.AddTransition(cond, nextState);

            return this;
        }
    }

    public static Builder GetNewBuilder()
    {
        return new Builder();
    }

    public CreatureStateMachine(BehaviorState initialState, int HP)
    {
        this.state = initialState;
        this.HP = HP;
        target = null;
    }

    public override void _PhysicsProcess(double delta)
    {
        BehaviorState nextState = state.StateTransition(this);

        //if not null, we transition!
        //if we need to listen in on this, put the event stuff here
        if (nextState != null)
        {
            state = nextState;
        }

        Velocity = state.GetStepVelocity(this);

        MoveAndSlide();
    }

    public void ChangeHP(int change, DamageSource source)
    {
        throw new NotImplementedException();
    }

    public int GetHP()
    {
        return HP;
    }

    public CreatureState GetState()
    {
        return state.GetCreatureState();
    }

    public void Push(Vector3 force)
    {
        Velocity += force;
    }

    public void Stun(float time)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetVelocity()
    {
        return Velocity;
    }

    public Vector3 GetPosition()
    {
        return Position;
    }
}
