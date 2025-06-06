using Godot;
using System;

/// <summary>
/// Interface that deals with conditions for state transitions for the CreatureStateMachine class.
/// </summary>
public interface BehaviorCondition {

    /// <summary>
    /// The target of the condition in a case where the condition is looking for a target. Not every conditional will return a target.
    /// </summary>
    /// <returns>The creature to target</returns>
    public ICreature GetTarget();

    /// <summary>
    /// Whether the condition has been met, typically will be referenced in regards to state transitions.
    /// </summary>
    /// <returns>The result of the conditional</returns>
    public bool Condition(CreatureStateMachine self);
}