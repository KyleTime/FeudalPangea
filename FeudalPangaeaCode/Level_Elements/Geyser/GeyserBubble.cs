using Godot;
using System;
using System.Threading.Tasks;

public partial class GeyserBubble : Node3D
{
    private float bottomY;
    [Export] Node3D scaleNode;
    [Export] Node3D platformPosition;
    [Export] private float riseFactor = 2;
    [Export] private float hangFactor = 0.2f;
    [Export] private float hangScaleLimit = 0.025f;
    [Export] private float bobDelay = 0;
    [Export] private float totalTime = 4;

    private float time = 0;

    public override void _Ready()
    {
        bottomY = scaleNode.GlobalPosition.Y - (Scale.Y * scaleNode.Scale.Y) / 2;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Time.GetTicksMsec() / 1000.0f < bobDelay)
        {
            return;
        }

        if (Mathf.Abs(Scale.Y - riseFactor) < riseFactor * hangScaleLimit || Mathf.Abs(Scale.Y) < riseFactor * hangScaleLimit)
        {
            time = (time + (float)delta * hangFactor) % totalTime;
        }
        else
        {
            time = (time + (float)delta) % totalTime;
        }

        scaleNode.Scale = scaleNode.Scale with { Y = ScaleSine(time, riseFactor) + 0.01f };

        scaleNode.GlobalPosition = scaleNode.GlobalPosition with { Y = bottomY + (Scale.Y * scaleNode.Scale.Y) / 2 };

        platformPosition.Position = new Vector3(0, scaleNode.Scale.Y - platformPosition.Scale.Y, 0);
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
