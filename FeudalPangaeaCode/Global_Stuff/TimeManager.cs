using Godot;

public partial class TimeManager : Node
{
    public static TimeManager stn;

    public static int freezeTimeCount = 0;
    public static bool gamePaused = false;

    /// <summary>
    /// Triggers whenever the freezeframe pause state changes.
    /// </summary>
    /// <param name="paused">Whether the game is currently in "freezeframe", also denoted by (freezeTimeCount != 0).
    /// Note that the game might be otherwise paused, so please check the GetTree().Paused property too</param>
    [Signal]
    public delegate void FreezeFrameEventHandler(bool paused);

    /// <summary>
    /// Triggers whenever the game is paused.
    /// </summary>
    /// <param name="paused">Whether the game is paused, also denoted by the (gamePaused) value</param>
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

        stn.GetTree().Paused = ShouldPause();

        stn.EmitSignal(SignalName.FreezeFrame, freezeTimeCount != 0);
    }

    public static void PauseTime(bool pause)
    {
        gamePaused = pause;

        bool shouldPause = ShouldPause();

        stn.GetTree().Paused = shouldPause;

        stn.EmitSignal(SignalName.Pause, shouldPause);
    }

    private static bool ShouldPause()
    {
        return gamePaused || freezeTimeCount != 0;
    }
}