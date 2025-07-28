using CreatureBehaviors.CreatureConditions;
using CreatureBehaviors.CreatureStates;
using Godot;
using System;

public partial class FireGoober : CreatureStateMachine, ICreature
{
    public override void _EnterTree()
    {
        CreatureStateMachine.GetNewBuilder()
            .AddState("Idle", new Idle(true, 100, 4))
            .AddState("Fireball", new ShootProjectile("fireballObject", 15, 10, 2))
            .AddState("Stun", new Idle(true))
            .AddState("Death", new DeathQueueFree())
            .SetHP(10)
            .SetStunState("Stun")
            .SetDeathState("Death")
            .AddTransition("Idle", new SeePlayerAtDistance(60), "Fireball")
            .AddTransition("Fireball", new StunOver(), "Idle")
            .buildOnExisting(this);
    }
}
