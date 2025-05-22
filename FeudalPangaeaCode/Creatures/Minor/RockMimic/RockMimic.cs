using Godot;
using System;

public partial class RockMimic : CreatureStateMachine
{
    public RockMimic(BehaviorState initialState, int HP) : base(initialState, HP)
    {
    } 
}