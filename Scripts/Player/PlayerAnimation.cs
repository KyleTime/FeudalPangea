using Godot;
using System;

public partial class PlayerAnimation : AnimationPlayer
{
    float speed = 1;
    float maxSpeed = 1;

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetMaxSpeed(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    public void UpdateAnimation(CreatureState state)
    {
        switch(state)
        {
            case CreatureState.Grounded:
                if(speed == 0)
                {
                    Play("Idle");
                }
                else
                    Play("Walk", -1, Mathf.Clamp(speed/maxSpeed, 0, 1));
                break;
            case CreatureState.Dive:
                Play("Dive");
                break;
            case CreatureState.OpenAir:
                Play("Air");
                break;
            case CreatureState.Attack:
                Play("Attack");
                break;
            case CreatureState.AttackAir:
                Play("Attack"); //TODO: Make an aerial attack
                break;
        }
    }
}
