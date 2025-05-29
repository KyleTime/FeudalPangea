using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class DeathSpawnRagdoll : BehaviorState
    {
        RigidBody3D ragdoll;

        public DeathSpawnRagdoll(RigidBody3D ragdoll) : base(CreatureState.Dead)
        {
            this.ragdoll = ragdoll;
            this.ragdoll.Freeze = true;
            //this.ragdoll.Visible = false;
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            ragdoll.GetParent().RemoveChild(ragdoll);
            self.GetTree().Root.AddChild(ragdoll);
            ragdoll.GlobalPosition = self.GetCreaturePosition();
            ragdoll.Freeze = false;
            ragdoll.Visible = true;

            self.QueueFree();

            return new Vector3();
        }
    }
}