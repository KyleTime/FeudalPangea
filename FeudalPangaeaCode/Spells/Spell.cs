using Godot;

namespace MagicSystem
{
    public abstract class Spell
    {
        protected PlayerMovement caster
        {
            get
            {
                return Player.player.move;
            }
        }

        public abstract void CastState(double delta);

        /// <summary>
        /// When the casting first begins.
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Start() { }
    }
}