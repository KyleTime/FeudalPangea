using Godot;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class HitFx : Node3D
{
    [Export] public Node3D effect;
    [Export] public float pauseTime = 1;
    [Export] public float step = 0.1f;
    [Export] public float scaleMod = 1;
    [Export] public bool freezeTime = true;
    private Vector3 initialScale;
    float time;

    public override void _Ready()
    {
        initialScale = effect.Scale.Normalized();
    }

    protected float InvertedParabolaScale(float x, float totalTime)
    {
        float w = totalTime / 2;
        return -1 * (float)Math.Pow((x - w) * (1 / w), 2) + 1;
    }

    public virtual async void Effect(Vector3 effectPos)
    {
        if (effect == null)
        {
            GD.PrintErr("Effect Node not set in HitFx!");
            return;
        }

        if(freezeTime)
            GetTree().Paused = true;

        effect.GlobalPosition = effectPos;
        effect.Visible = true;

        while (time < pauseTime)
        {
            ChangeEffectAtStep();

            time += step;
            await Task.Delay((int)(step * 1000));
        }

        if(freezeTime)
            GetTree().Paused = false;

        effect.Visible = false;

        time = 0;
    }

    protected virtual void ChangeEffectAtStep()
    {
        effect.Scale = initialScale * InvertedParabolaScale(time, pauseTime) * scaleMod;
    }
}