using Godot;
using MagicSystem;

namespace CreatureBehaviors.CreatureStates
{
    public class ShootProjectile : BehaviorState
    {
        ObjectPool projectilePool;
        float launchSpeed;
        int damage;
        float raise;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectileName">Projectile to fire, MUST BE OF TYPE 'AimedProjectile'</param>
        /// <param name="stateOverride"></param>
        public ShootProjectile(string projectileName, float launchSpeed = 5, int damage = 10, float raise = 1, CreatureState stateOverride = CreatureState.Casting) : base(stateOverride)
        {
            projectilePool = new ObjectPool(GlobalData.GetPackedScene(projectileName), 2);
            this.launchSpeed = launchSpeed;
            this.damage = damage;
            this.raise = raise;
        }

        public override void HandleAnimation(AnimationPlayer player)
        {
        }

        public override Vector3 TransitionIn(CreatureStateMachine self, double delta)
        {
            return base.TransitionIn(self, delta);
        }

        public override Vector3 GetStepVelocity(CreatureStateMachine self, double delta)
        {
            AimedProjectile projectile = (AimedProjectile)projectilePool.GetCurrentObject();

            projectile.hitbox.dmg = damage;

            Vector3 directionToTarget = (self.target.GetCreaturePosition() - self.GetCreaturePosition()).Normalized();

            projectile.FireProjectile(self, self.GlobalPosition + new Vector3(0, raise, 0), directionToTarget * launchSpeed);

            return new Vector3();
        }
    }
}