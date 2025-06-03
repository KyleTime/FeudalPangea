using Godot;
using System;
using System.Collections.Generic;


public partial class CreatureStateMachine : CharacterBody3D, ICreature
{
    private BehaviorState state;

    //when the creature is stunned, which state should it transition to?
    private BehaviorState stunState;
    public float stunTimer = 0; //how much time is left to be stunned?

    //what should happen when the creature dies?
    private BehaviorState deathState;

    //the creature that this creature is targetting (for whatever reason or purpose)
    public ICreature target;

    private int HP = 1;

    private double deltaTime;

    //offset from feet to the center of the creature
    [Export] public Vector3 creatureCenterOffset = new Vector3(0, 1, 0);

    //This is the builder for the CreatureStateMachine
    // refer to the following link for a resource on the Builder Pattern: https://www.baeldung.com/java-builder-pattern 
    // yes, it's in java, but that's how my classes taught me so anyway
    public class Builder
    {
        int HP = 1;
        BehaviorState initialState;
        BehaviorState stunState;
        BehaviorState deathState;

        Dictionary<string, BehaviorState> states = new Dictionary<string, BehaviorState>();

        public CreatureStateMachine build()
        {

            //something that's kinda funny is that we don't actually *need* to give 
            //CreatureStateMachine the whole dictionary. All of the states are already linked together!
            //This also neatly culls any unused states from memory, which is nice.

            return new CreatureStateMachine(initialState, HP, stunState, deathState);
        }

        public void buildOnExisting(CreatureStateMachine machine)
        {
            machine.state = initialState;
            machine.stunState = stunState;
            machine.deathState = deathState;
            machine.HP = HP;
            machine.target = null;
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
        /// Set the state that the creature should transition to whenever it's stunned.
        /// Ideally, the state should reference the "stunTimer" in CreatureStateMachine.
        /// </summary>
        /// <param name="stateName">Name of the state</param>
        /// <returns>The Builder!</returns>
        public Builder SetStunState(string stateName)
        {
            stunState = states[stateName];
            return this;
        }

        /// <summary>
        /// Set the state the creature should transition to whenever it dies.
        /// </summary>
        /// <param name="stateName">Name of the state to transition to.</param>
        /// <returns>The Builder!</returns>
        public Builder SetDeathState(string stateName)
        {
            deathState = states[stateName];
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

    [Signal]
    public delegate void HPChangeEventHandler(int old, int cur, DamageSource source);

    [Signal]
    public delegate void StateChangeEventHandler(string prevState, string nextState);

    public static Builder GetNewBuilder()
    {
        return new Builder();
    }

    public CreatureStateMachine(BehaviorState initialState, int HP, BehaviorState stunState, BehaviorState deathState)
    {
        this.state = initialState;
        this.stunState = stunState;
        this.deathState = deathState;
        this.HP = HP;
        target = null;

        if (deathState == null)
        {
            GD.Print("WHAT!");
        }
        else
        {
            GD.Print("death assigned properly!");
        }
    }

    public CreatureStateMachine()
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        deltaTime = delta;

        stunTimer = Math.Clamp(stunTimer - (float)delta, 0, 9999);

        BehaviorState nextState = state.StateTransition(this, delta);

        //if not null, we transition!
        //if we need to listen in on this, put the event stuff here
        if (nextState != null)
        {
            string stateName = state.name;
            state = nextState;
            EmitSignal(SignalName.StateChange, stateName, state.name);
        }

        Velocity = state.GetStepVelocity(this, delta);

        MoveAndSlide();
    }

    public void ChangeHP(int change, DamageSource source)
    {
        HP += change;
        EmitSignal(SignalName.HPChange, HP - change, HP, (int)source);

        if (HP <= 0)
        {
            GD.Print("DEAD!");

            if (deathState == null)
                GD.Print("death is null");

            ForceState(deathState, deltaTime, null, true);
        }
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
        stunTimer = time;
        ForceState(stunState, deltaTime);
        target = null; //reset target on stun prolly
    }

    /// <summary>
    /// Force a state transition with a given state and target, bypassing the conditional.
    /// However, this does not bypass death unless otherwise specified.
    /// </summary>
    /// <param name="nextState">State to transition to.</param>
    /// <param name="target">The creature to target.</param>
    public void ForceState(BehaviorState nextState, double delta, ICreature target = null, bool bypassDeath = false)
    {
        if (!bypassDeath && HP <= 0)
        {
            return;
        }

        if (target != null)
        {
            this.target = target;
        }

        state.TransitionOut(this, delta);

        state = nextState;

        state.TransitionIn(this, delta);
    }

    /// <summary>
    /// Pipe GetVelocity() into GetCreatureVelocity() so that mistakes are caught
    /// </summary>
    /// <returns>Creature Velocity</returns>
    public new Vector3 GetVelocity()
    {
        return GetCreatureVelocity();
    }

    /// <summary>
    /// Pipe GetPosition() into GetCreaturePosition() so that mistakes are caught
    /// </summary>
    /// <returns>Creature Velocity</returns>
    public new Vector3 GetPosition()
    {
        return GetCreaturePosition();
    }

    public Vector3 GetCreatureVelocity()
    {
        return Velocity;
    }

    public Vector3 GetCreaturePosition()
    {
        return GlobalPosition;
    }

    public Vector3 GetCreatureCenter()
    {
        return GlobalPosition + creatureCenterOffset;
    }
}
