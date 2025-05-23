using Godot;

//state that has the creature waiting for some stimuli
public class Idle : BehaviorState
{
    public bool doGravity;

    public Idle(bool doGravity) : base(CreatureState.Grounded)
    {
        this.doGravity = doGravity;
    }

    public override Vector3 GetStepVelocity(CreatureStateMachine self)
    {
        return new Vector3();
    }
}