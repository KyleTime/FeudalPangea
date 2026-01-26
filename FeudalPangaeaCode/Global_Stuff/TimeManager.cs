using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class TimeManager : Node
{
    public static TimeManager stn;

    static int freezeTimer;

    static bool gamePaused = false;
    public static bool gameIsPaused
    {
        private set { gamePaused = value; }
        get { return gamePaused; }
    }

    /// <summary>
    /// Triggers whenever the freezeframe pause state changes.
    /// Freezes the game, useful for impact stuff.
    /// </summary>
    /// <param name="paused">Whether the game is currently in "freezeframe", also denoted by (freezeTimeCount != 0).
    /// Note that the game might be otherwise paused, so please check the GetTree().Paused property too</param>
    [Signal]
    public delegate void FreezeFrameEventHandler(bool paused);

    /// <summary>
    /// Triggers whenever the game is paused.
    /// Higher priority than freeze frame, if this says the game is paused, it's paused until this says so.
    /// </summary>
    /// <param name="paused">Whether the game is paused, also denoted by the (gamePaused) value</param>
    [Signal]
    public delegate void PauseEventHandler(bool paused);

    public override void _Ready()
    {
        stn = this;
    }

    static IEnumerable<int> HandleFrozenTime()
    {
        while(freezeTimer > 0)
        {
            yield return 100;
            freezeTimer -= 100;
        }

        freezeTimer = 0;
        stn.GetTree().Paused = ShouldPause();
    }

    //for things that need to temporarily freeze the game for like, impact and stuff
    public static void FreezeTime(int hangTime)
    {
        bool timeGoing = freezeTimer == 0;

        freezeTimer += hangTime;

        stn.GetTree().Paused = ShouldPause();

        stn.EmitSignal(SignalName.FreezeFrame, true);

        if(timeGoing)
            Coroutine.StartCoroutine(HandleFrozenTime());
    }

    //for menus that fully pause the game
    public static void PauseTime(bool pause)
    {
        gamePaused = pause;

        bool shouldPause = ShouldPause();

        stn.GetTree().Paused = shouldPause;

        stn.EmitSignal(SignalName.Pause, shouldPause);
    }

    private static bool ShouldPause()
    {
        return gamePaused || freezeTimer > 0;
    }
}