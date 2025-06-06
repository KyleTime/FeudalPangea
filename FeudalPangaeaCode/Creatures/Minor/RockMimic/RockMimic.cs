using Godot;
using System;
using CreatureBehaviors.CreatureStates;
using CreatureBehaviors.CreatureConditions;


public partial class RockMimic : CreatureStateMachine, ICreature
{
    [Export] RagdollCreature ragdoll;
    [Export] Hitbox hitbox;

    public override void _Ready()
    {
        CreatureStateMachine.GetNewBuilder()
                        .AddState("Follow", new FollowCreature(20, 10, 5))
                        .AddState("Stun", new Idle(true))
                        .AddState("Idle", new Idle(true))
                        .AddState("Reset", new Idle(true, 10, 3))
                        .AddState("Death", new DeathSpawnRagdoll(ragdoll))
                        .AddState("Attack", new JumpAtCreature([hitbox], 20, 5, 10))
                        .SetHP(20)
                        .SetDeathState("Death")
                        .SetInitialState("Idle")
                        .SetStunState("Stun")
                        .AddTransition("Stun", new StunOver(), "Idle")
                        .AddTransition("Idle", new SeePlayerAtDistance(-1, 2), "Follow")
                        .AddTransition("Follow", new SeePlayerAtDistance(10), "Attack")
                        .AddTransition("Attack", new HitCreature([hitbox]), "Reset")
                        .AddTransition("Attack", new StunOver(), "Idle")
                        .AddTransition("Reset", new StunOver(), "Idle")
                        .buildOnExisting(this);
    }

}