using Godot;
using System;


public class FollowCreature : BehaviorState
{
    public FollowCreature():base(CreatureState.Grounded) {}

    public override Vector3 GetStepVelocity(CreatureStateMachine self)
    {
        throw new NotImplementedException();
    }
}