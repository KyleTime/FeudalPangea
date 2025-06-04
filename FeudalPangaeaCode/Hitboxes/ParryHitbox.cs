using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;

public partial class ParryHitbox : Hurtbox
{
    [Export] public Node3D effect;
    [Export] public float horizontalPush = 1;
    [Export] public float verticalPush = 1;

    public override void HitBy(Hitbox hitbox)
    {
        if (hitbox.Owner is not ICreature)
            return;

        ICreature creature = (ICreature)hitbox.Owner;

        creature.Stun(GlobalData.parryStunTime);

        CreatureVelocityCalculations.PushCreature(self, creature, horizontalPush);
        creature.Push(Vector3.Up * verticalPush);

        GD.Print("PARRY!");

        if (effect != null)
        {
            ParryPause(1, hitbox.GlobalPosition);
        }
    }

    private float InvertedParabolaScale(float x, float totalTime) {
        float w = totalTime / 2;
        return -1 * (float)Math.Pow((x - w) * (1 / w), 2) + 1;
    }

    private async void ParryPause(float pauseTime, Vector3 effectPos)
    {
        GetTree().Paused = true;

        effect.GlobalPosition = effectPos;
        effect.Visible = true;

        float time = 0;
        float step = 0.1f;
        while (time < pauseTime)
        {
            effect.Scale = new Vector3(1, 1, 1) * InvertedParabolaScale(time, pauseTime);

            time += step;
            await Task.Delay((int)(step * 1000));
        }

        GetTree().Paused = false;

        effect.Visible = false;
    }

    public bool OverlapsHitbox()
    {
        Array<Area3D> array = GetOverlappingAreas();

        foreach (Area3D area in array)
        {
            if (area.Owner != Owner && area is Hitbox && area.Owner is ICreature)
            {
                return true;
            }
        }

        return false;
    }
}