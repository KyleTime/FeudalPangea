using Godot;

namespace MagicSystem
{
    public partial class AimedProjectile : Node3D
    {
        public bool active { get; private set; }
        public Vector3 velocity;

        /// <summary>
        /// Hitbox that detects when projectile hits the ground to stop it
        /// </summary>
        [Export] Area3D groundbox;

        /// <summary>
        /// Hitbox that projectile uses to deal damage
        /// </summary>
        [Export] public Hitbox hitbox;

        #region Camera Alignment Aiming Variables
        /// <summary>
        /// target point to join camera middle
        /// </summary>
        Vector3 target;
        /// <summary>
        /// direction to switch to once aligned
        /// </summary>
        Vector3 direction;
        /// <summary>
        /// speed to move at
        /// </summary>
        float speed;
        /// <summary>
        /// whether the projectile is currently aligning
        /// </summary>
        bool aligning = false;
        #endregion

        public override void _Ready()
        {
            groundbox.BodyShapeEntered += CollideWithGround;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (aligning)
            {
                if (KMath.Dist3D(target, GlobalPosition) < 1)
                {
                    velocity = direction.Normalized() * speed;
                    aligning = false;
                }

                velocity = (target - GlobalPosition).Normalized() * speed;
            }

            Position += velocity * (float)delta;
        }

        /// <summary>
        /// Apply some push force to the projectile
        /// </summary>
        /// <param name="factor"></param>
        public void Push(Vector3 factor)
        {
            aligning = false;
            velocity += factor;
        }

        /// <summary>
        /// Fire the projectile from the player with automatic direction.
        /// This will spawn the projectile at the player position and set 
        /// up the aim assist automatically.
        /// </summary>
        /// <param name="caster">The player in question</param>
        /// <param name="speed">The speed of the projectile</param>
        public void FireProjectileFromPlayer(PlayerMovement caster, float speed)
        {
            FireProjectileFromPlayer(caster, -caster.basis.Z, speed);
        }

        /// <summary>
        /// Fire the projectile from the player given a direction, speed, and potentially target distance.
        /// The hitbox of the projectile will be set not to hit the player immediately to avoid self-damage.
        /// Otherwise the projectile is set up with aim assist using the given direction.
        /// </summary>
        /// <param name="caster">The player casting this</param>
        /// <param name="direction">The direction to fire it in</param>
        /// <param name="speed">The speed at which to travel</param>
        /// <param name="targetDistance">How far along the view vector this projectile should meet up with it.</param>
        public void FireProjectileFromPlayer(PlayerMovement caster, Vector3 direction, float speed, float targetDistance = 40)
        {
            GlobalPosition = caster.GlobalPosition;
            hitbox.ignore = Player.player;
            this.direction = direction;
            float camHeight = Player.player.cam.GlobalPosition.Y - caster.GlobalPosition.Y;
            target = direction.Normalized() * targetDistance + new Vector3(0, camHeight, 0) + caster.GlobalPosition;
            this.speed = speed;

            Activate(true);
        }

        /// <summary>
        /// A general purpose projectile firing function.
        /// Does not use any kind of aim assist, and simply sets the position and
        /// velocity of the projectile.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public void FireProjectile(ICreature caster, Vector3 position, Vector3 velocity)
        {
            GlobalPosition = position;
            hitbox.ignore = caster;
            this.velocity = velocity;

            Activate(false);
        }

        /// <summary>
        /// 'Activates' the projectile by initializing vital variables.
        /// Largely just makes it visible, tells the projectile that it's active, and
        /// enables the hitbox.
        /// </summary>
        /// <param name="target">Whether the projectile should use aim assist.</param>
        public void Activate(bool target = false)
        {
            active = true;
            hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
            Visible = true;
            hitbox.hitsPlayer = true;
            aligning = target;
        }

        /// <summary>
        /// When the ground hitbox collides with something, this will trigger.
        /// Main purpose is to disable the projectile as soon as it hits the ground.
        /// </summary>
        /// <param name="bodyRid"></param>
        /// <param name="body"></param>
        /// <param name="bodyShapeIndex"></param>
        /// <param name="localShapeIndex"></param>
        private void CollideWithGround(Rid bodyRid, Node3D body, long bodyShapeIndex, long localShapeIndex)
        {
            Deactivate();
        }

        public void Deactivate()
        {
            active = false;
            Visible = false;
            velocity = new Vector3();
            aligning = false;
            hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
        }
    }
}