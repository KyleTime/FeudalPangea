using Godot;
using System;

public partial class PlayerAnimation : Node
{
    float speed = 1;
    float maxSpeed = 1;

    public AnimationPlayer anim;

    public override void _Ready()
	{
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
    }

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
                    anim.Play("Idle");
                }
                else
                    anim.Play("Run", -1, Mathf.Clamp(speed/maxSpeed, 0, 1));
                break;
            case CreatureState.Dive:
                anim.Play("Dive");
                break;
            case CreatureState.OpenAir:
                anim.Play("Air");
                break;
            case CreatureState.Dead:
                anim.Play("Dive"); //TODO: Make a death animation
                break;
            case CreatureState.LedgeHang:
                anim.Play("Hang");
                break;
        }
    }

    public void AnimationOverride(string animName){
        anim.Play(animName);
    }
}
