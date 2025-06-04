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
                        .AddState("Follow", new FollowCreature(3, 1, 5))
                        .AddState("Stun", new Idle(true))
                        .AddState("Idle", new Idle(true))
                        .AddState("Reset", new Idle(true, 5, 3))
                        .AddState("Death", new DeathSpawnRagdoll(ragdoll))
                        .AddState("Attack", new JumpAtCreature(hitbox))
                        .SetHP(20)
                        .SetDeathState("Death")
                        .SetInitialState("Idle")
                        .SetStunState("Stun")
                        .AddTransition("Stun", new StunOver(), "Idle")
                        .AddTransition("Idle", new SeePlayerAtDistance(10, 2), "Follow")
                        .AddTransition("Follow", new SeePlayerAtDistance(5), "Attack")
                        .AddTransition("Attack", new HitCreature([hitbox]), "Reset")
                        .AddTransition("Reset", new StunOver(), "Idle")
                        .buildOnExisting(this);
    }

}