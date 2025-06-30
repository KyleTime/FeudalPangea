using Godot;

namespace MagicSystem
{
    public class FireballSpell : Spell
    {
        public ObjectPool pool;
        const float speed = 40;
        const int numFireballs = 3;

        float minimumStun = 1;
        float timer = 0;

        public FireballSpell()
        {
        }

        public override void Start()
        {
            FireballObject fireball = GetFireball();

            if (fireball != null)
            {
                timer = 0;
                fireball.FireProjectileFromPlayer(caster, speed);

                caster.RotateBody();
                caster.PlayAnimation("Dive");
                caster.WaitForAnimation();
                caster.Push(caster.basis.Z * 10);
            }
            else
            {
                caster.animationDone = true;
            }
        }

        public override void CastState(double delta)
        {
            if (caster.animationDone && timer > minimumStun)
            {
                caster.TryTransition(caster.GroundedCond(), CreatureState.Grounded);
                caster.TryTransition(caster.OpenAirCond(), CreatureState.OpenAir);
            }

            timer += (float)delta;
        }

        public FireballObject GetFireball()
        {
            if (pool == null)
            {
                pool = new ObjectPool(GlobalData.GetPackedScene("fireballObject"), numFireballs);
            }

            FireballObject fireballObject = pool.GetCurrentObject() as FireballObject;

            for(int i = 0; i < numFireballs; i++)
            {
                if (!fireballObject.active)
                {
                    return fireballObject;
                }

                fireballObject = pool.GetNext() as FireballObject;
            }

            return null;
        }
    }
}