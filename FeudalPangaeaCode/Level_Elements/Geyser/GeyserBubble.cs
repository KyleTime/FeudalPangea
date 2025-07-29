using Godot;
using System;
using System.Threading.Tasks;

public partial class GeyserBubble : Node3D
{
    private float bottomY;
    [Export] private float riseFactor = 2;
    [Export] private float hangFactor = 0.2f;
    [Export] private float hangScaleLimit = 0.05f;
    [Export] private int bobOrder = 0;
    [Export] private float totalTime = 4;
    static float frameDelay = 0.01f;
    static int msDelay = (int)(frameDelay * 1000);


    public override void _Ready()
    {
        bottomY = GlobalPosition.Y - Scale.Y / 2;

        BobRoutine();
    }

    private async void BobRoutine()
    {
        await Task.Delay(bobOrder * 2000);

        float time = 0;
        while (true)
        {
            if (!IsInstanceValid(GetTree()))
            {
                return;
            }

            if (GetTree().Paused)
            {
                await Task.Delay(msDelay);
                continue;
            }

            if (Mathf.Abs(Scale.Y - riseFactor) < riseFactor * hangScaleLimit || Mathf.Abs(Scale.Y) < riseFactor * hangScaleLimit)
            {
                time = (time + frameDelay * hangFactor) % totalTime;
            }
            else
            {
                time = (time + frameDelay) % totalTime;
            }

            Scale = Scale with { Y = ScaleSine(time, riseFactor) + 0.01f };

            GlobalPosition = GlobalPosition with { Y = bottomY + Scale.Y / 2 };

            await Task.Delay(msDelay);
        }
    }

    /// <summary>
    /// Calculates the scale that the geyser should follow based on a couple variables.
    /// </summary>
    /// <param name="t">Current time of the scale</param>
    /// <param name="h">The maximum height the geyser should reach</param>
    /// <returns>The current height of the geyser</returns>
    private float ScaleSine(float t, float h)
    {
        return Mathf.Sin(((4 * t - totalTime) * 2 * Mathf.Pi) / (4 * totalTime)) * h / 2 + h / 2;
    }
}
