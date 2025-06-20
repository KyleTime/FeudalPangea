using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Interface to be used by the CreatureStateMachine class for state behavior.
/// </summary>
public abstract class BehaviorState
{

    public string name = "name";

    //NOTE: the state here doesn't have any bearing on the state, it exists primarily to classify the state
    protected readonly CreatureState creatureState;

    protected Dictionary<BehaviorCondition, BehaviorState> transitions = new Dictionary<BehaviorCondition, BehaviorState>();

    protected bool holdTransitions = false;

    public BehaviorState(CreatureState creatureState)
    {
        this.creatureState = creatureState;
    }

    /// <summary>
    /// Add a transition to the dictionary.
    /// </summary>
    /// <param name="cond">Condition to check for.</param>
    /// <param name="state">Transition to swap to.</param>
    public void AddTransition(BehaviorCondition cond, BehaviorState state)
    {
        transitions.Add(cond, state);
    }

    /// <summary>
    /// Gets the state that this BehaviorState implementation represents.
    /// </summary>
    /// <returns>Corresponding CreatureState</returns>
    public CreatureState GetCreatureState()
    {
        return creatureState;
    }

    /// <summary>
    /// Runs the state logic and returns the new velocity after modifications.
    /// </summary>
    /// <param name="self">The CreatureStateMachine this state is acting on</param>
    /// <param name="delta">Delta Time</param>
    /// <returns>The new velocity.</returns>
    public abstract Vector3 GetStepVelocity(CreatureStateMachine self, double delta);

    /// <summary>
    /// Feed the state an animation player to do basically whatever with.
    /// </summary>
    /// <param name="player">The animation player in question</param>
    public abstract void HandleAnimation(AnimationPlayer player);

    /// <summary>
    /// Called on the first frame of the state.
    /// </summary>
    /// <param name="self">The CreatureStateMachine this state is acting on.</param>
    /// <param name="delta">Delta Time</param>
    /// <returns></returns>
    public virtual Vector3 TransitionIn(CreatureStateMachine self, double delta)
    {
        return self.Velocity;
    }

    /// <summary>
    /// Checks the condition for each state transition until it finds a valid one. Then, it attempts to extract a target and assign it to the new state.
    /// </summary>
    /// <returns>The new state to use</returns>
    public virtual BehaviorState StateTransition(CreatureStateMachine self, double delta)
    {
        if (holdTransitions)
            return null;

        foreach (var pair in transitions)
            {
                if (pair.Key.Condition(self))
                {
                    GD.Print("State: " + pair.Value.GetType().Name);

                    //transition out of this state
                    TransitionOut(self, delta);

                    //grab target from conditional
                    //if null, assume no change
                    ICreature target = pair.Key.GetTarget();
                    if (target != null)
                    {
                        self.target = target;
                    }

                    //transition into new state
                    self.Velocity = pair.Value.TransitionIn(self, delta);

                    return pair.Value;
                }
            }

        return null;
    }

    /// <summary>
    /// Function called when transitioning out of the current state.
    /// Useful for cleanup.
    /// </summary>
    /// <param name="self">The CreatureStateMachine this is attached to.</param>
    /// <param name="delta">Delta time</param>
    /// <returns>New velocity after end of transition.</returns>
    public virtual Vector3 TransitionOut(CreatureStateMachine self, double delta)
    {
        return self.Velocity;
    }
}