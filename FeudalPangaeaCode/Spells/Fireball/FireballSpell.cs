using Godot;

namespace MagicSystem
{
    public class FireballSpell : Spell
    {
        public FireballObject fireball;

        const float speed = 10;

        public Vector3 camLine;
        public bool alignWithCamera = true;
        float camHeight = 0;
        const float targetDistance = 40;

        public FireballSpell()
        {
        }

        public override void Start()
        {
            fireball = GetFireball();

            fireball.GlobalPosition = caster.GlobalPosition;

            if (alignWithCamera)
            {
                camLine = -caster.basis.Z;
                camHeight = Player.player.cam.GlobalPosition.Y - caster.GlobalPosition.Y;

                Vector3 d1 = camLine.Normalized() * targetDistance + new Vector3(0, camHeight, 0);

                Vector3 direction = camLine;
                Vector3 target = d1 + caster.GlobalPosition;

                fireball.Activate(direction, speed, target);
            }
            else
            {
                fireball.velocity = -caster.basis.Z * speed;
                fireball.Activate();
            }

            caster.PlayAnimation("Dive");
            caster.WaitForAnimation();
        }

        public override void CastState(double delta)
        {
            if (caster.animationDone)
            {
                caster.TryTransition(caster.GroundedCond(), CreatureState.Grounded);
                caster.TryTransition(caster.OpenAirCond(), CreatureState.OpenAir);
            }
        }

        public FireballObject GetFireball()
        {
            if (fireball == null)
            {
                PackedScene fireballScene = GlobalData.GetPackedScene("fireballObject");

                FireballObject fireballObject = (FireballObject)fireballScene.Instantiate();

                caster.GetTree().Root.AddChild(fireballObject);

                return fireballObject;
            }

            return fireball;
        }
    }
}