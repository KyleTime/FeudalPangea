using Godot;
using System;
using CreatureBehaviors.CreatureStates;
using CreatureBehaviors.CreatureConditions;


public partial class RockMimic : CreatureStateMachine, ICreature
{
    [Export] RigidBody3D ragdoll;

    public override void _Ready()
    {
        CreatureStateMachine.GetNewBuilder()
                        .AddState("Follow", new FollowCreature(3, 1, 5))
                        .AddState("Stun", new Idle(true))
                        .AddState("Idle", new Idle(true))
                        .AddState("Death", new DeathSpawnRagdoll(ragdoll))
                        .SetHP(10)
                        .SetDeathState("Death")
                        .SetInitialState("Idle")
                        .SetStunState("Stun")
                        .AddTransition("Stun", new StunOver(), "Idle")
                        .AddTransition("Idle", new SeePlayerAtDistance(10, 2), "Follow")
                        .buildOnExisting(this);
    }

}