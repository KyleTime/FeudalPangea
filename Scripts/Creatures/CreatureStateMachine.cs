using Godot;
using System;

public partial class CreatureStateMachine : CharacterBody3D, ICreature
{
    private BehaviorState state;

    private int HP = 1;

    public CreatureStateMachine(){
        
    }

    public override void _Process(double delta){

    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = state.GetStepVelocity(this);

        MoveAndSlide();
    }

    public void ChangeHP(int change, DamageSource source)
    {
        throw new NotImplementedException();
    }

    public int GetHP()
    {
        return HP;
    }

    public CreatureState GetState()
    {
        return state.GetCreatureState();
    }

    public void Push(Vector3 force)
    {
        Velocity += force;
    }

    public void Stun(float time)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetVelocity(){
        return Velocity;
    }


}
