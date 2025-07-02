using Godot;

public partial class TimeManager : Node
{
    public static TimeManager stn;

    public static int freezeTimeCount;

    [Signal]
    public delegate void FreezeFrameEventHandler(bool paused);

    [Signal]
    public delegate void PauseEventHandler(bool paused);

    public override void _Ready()
    {
        stn = this;
    }

    public static void FreezeTime(bool freeze)
    {
        if (freeze)
        {
            freezeTimeCount++;
        }
        else
        {
            freezeTimeCount--;
        }

        freezeTimeCount = Mathf.Clamp(freezeTimeCount, 0, 999);

        bool timeFrozen = !(freezeTimeCount == 0);

        stn.GetTree().Paused = timeFrozen;

        stn.EmitSignal(SignalName.FreezeFrame, timeFrozen);
    }

    public static void PauseTime(bool pause)
    {
        stn.GetTree().Paused = pause;
        stn.EmitSignal(SignalName.Pause, pause);
    }
}