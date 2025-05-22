using Godot;
using System;


public class FollowCreature : BehaviorState
{
    float acceleration;
    float maxSpeed;
    float jumpPower;

    //constructor with
    public FollowCreature(float acceleration, float maxSpeed, float jumpPower) : base(CreatureState.Grounded)
    {
        SetUp(acceleration, maxSpeed, jumpPower);
    }

    public FollowCreature(float acceleration, float maxSpeed) : base(CreatureState.Grounded)
    {
        SetUp(acceleration, maxSpeed, 0);
    }

    private void SetUp(float acceleration, float maxSpeed, float jumpPower)
    {
        this.acceleration = acceleration;
        this.maxSpeed = maxSpeed;
    }

    public override Vector3 GetStepVelocity(CreatureStateMachine self)
    {
        Vector3 velocity = self.GetVelocity();

        return velocity;
    }
}