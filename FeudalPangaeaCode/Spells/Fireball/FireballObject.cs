using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace MagicSystem
{
    public partial class FireballObject : AimedProjectile
    {
        public const float FireballLifeTime = 5;
        float timer = 0;

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (active)
            {
                timer += (float)delta;
                if (timer > FireballLifeTime)
                {
                    Deactivate();
                }
            }
            else
            {
                timer = 0;
            }
        }

    }
}