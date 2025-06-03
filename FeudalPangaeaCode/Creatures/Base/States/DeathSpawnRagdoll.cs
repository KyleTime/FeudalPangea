using Godot;

namespace CreatureBehaviors.CreatureStates
{

    //state that has the creature waiting for some stimuli
    //pretty good for a stun state actually
    public class DeathSpawnRagdoll : BehaviorState
    {
        RagdollCreature ragdoll;

        public DeathSpawnRagdoll(RagdollCreature ragdoll) : base(CreatureState.Dead)
        {
            this.ragdoll = ragdoll;
            this.ragdoll.Freeze = true;
            //this.ragdoll.Visible = false;
        }

        public override void HandleAnimation(AnimationPlayer player)
        {  
        }

        public override Vector3 TransitionIn(CreatureStateMachine self, double delta)
        {
            ragdoll.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
            ragdoll.hurtbox.SetDeferred(Area3D.PropertyName.Monitoring, false);
            ragdoll.hurtbox.SetDeferred(Area3D.PropertyName.Monitorable, false);

            return base.TransitionIn(self, delta);
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            ragdoll.GetParent().RemoveChild(ragdoll);
            self.GetTree().Root.AddChild(ragdoll);
            ragdoll.GlobalPosition = self.GetCreaturePosition();
            ragdoll.Freeze = false;
            ragdoll.Visible = true;

            ragdoll.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
            ragdoll.hurtbox.SetDeferred(Area3D.PropertyName.Monitoring, true);
            ragdoll.hurtbox.SetDeferred(Area3D.PropertyName.Monitorable, true);


            self.QueueFree();

            return new Vector3();
        }
    }
}