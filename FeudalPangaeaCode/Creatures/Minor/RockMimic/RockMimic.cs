using Godot;
using System;
using CreatureBehaviors.CreatureStates;
using CreatureBehaviors.CreatureConditions;


public partial class RockMimic : CreatureStateMachine, ICreature
{
    public RockMimic() : base()
    {
        CreatureStateMachine.GetNewBuilder()
                        .AddState("Follow", new FollowCreature(2, 10, 5))
                        .AddState("Stun", new Idle(true))
                        .AddState("Idle", new Idle(false))
                        .SetInitialState("Idle")
                        .SetStunState("Stun")
                        .AddTransition("Stun", new StunOver(), "Idle")
                        .AddTransition("Idle", new SeePlayerAtDistance(), "Follow")
                        .buildOnExisting(this);
    }
}