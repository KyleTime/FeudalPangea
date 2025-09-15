using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

    private bool pause;

    public async override void _Ready()
    {
        if (effect == null)
        {
            effect = GetChild<Node3D>(0);
        }

        effect.Visible = false;

        initialScale = effect.Scale.Normalized();

        await Task.Delay(1);

        TimeManager.stn.Pause += Pause;
    }

    protected float InvertedParabolaScale(float x, float totalTime)
    {
        float w = totalTime / 2;
        return -1 * (float)Math.Pow((x - w) * (1 / w), 2) + 1;
    }

    public virtual async void Effect(Vector3 effectPos, Vector3 rotation)
    {
        if (effect == null)
        {
            GD.PrintErr("Effect Node not set in HitFx!");
            return;
        }

        if (freezeTime)
            FreezeFrame(true);

        effect.GlobalPosition = effectPos;
        effect.Rotation = rotation with { X = 0, Z = 0 };
        effect.Visible = true;

        while (time < pauseTime)
        {
            if (pause)
            {
                await Task.Delay(100);
                continue;
            }

            ChangeEffectAtStep();

            time += step;
            await Task.Delay((int)(step * 1000));
        }

        if (freezeTime)
            FreezeFrame(false);

        effect.Visible = false;

        time = 0;
    }

    protected virtual void ChangeEffectAtStep()
    {
        effect.Scale = initialScale * InvertedParabolaScale(time, pauseTime) * scaleMod;
    }

    protected void FreezeFrame(bool freeze)
    {
        TimeManager.FreezeTime(freeze);
    }

    private void Pause(bool pause)
    {
        this.pause = pause;
    }
}