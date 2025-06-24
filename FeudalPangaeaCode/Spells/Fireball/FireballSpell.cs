using Godot;

namespace MagicSystem
{
    public class FireballSpell : Spell
    {
        public FireballObject fireball;

        const float speed = 5;

        public override void Start()
        {
            fireball = GetFireball();
            fireball.Activate();

            fireball.GlobalPosition = caster.GlobalPosition;
            fireball.velocity = -caster.basis.Z * speed;
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