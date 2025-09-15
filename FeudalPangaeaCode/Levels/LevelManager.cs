using Godot;
using MagicSystem;
using System;
using System.Threading.Tasks;

public partial class LevelManager : Node
{
    public static LevelManager currentLevel;

    /// <summary>
    /// When the player spawns, where should they be?
    /// </summary>
    [Export] public Vector3 startPos = new Vector3();
    [Export] public Vector3 startRot = new Vector3();

    [Export] public ulong tickStartTime;

    [Signal]
    public delegate void ResetLevelEventHandler();

    [Signal]
    public delegate void WinLevelEventHandler();

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Pausable;
        currentLevel = this;

        if (!HasNode("FatherTime"))
        {
            Node time = GlobalData.GetPackedScene("fatherTime").Instantiate();
            AddChild(time);
        }

        tickStartTime = Time.GetTicksMsec();
    }

    /// <summary>
    /// Basically just unfreezes time and sends out a signal to reload the level
    /// Mainly, the signal is listened to by things like Anchors and the Player to
    /// know when to reset themselves.
    /// </summary>
    public void ReloadLevel()
    {
        TimeManager.PauseTime(false);
        EmitSignal(SignalName.ResetLevel);
    }

    /// <summary>
    /// Send out a signal that the player has won the level
    /// Also freeze-frames the game and disables the pause menu
    /// </summary>
    /// <param name="nextScene">The scene to transition to after the win stuff is done</param>
    /// <param name="msDelay">Millisecond delay before loading the next scene</param>
    public async void Win(PackedScene nextScene, int msDelay)
    {
        EmitSignal(SignalName.WinLevel);
        await Task.Delay(msDelay);
        GetTree().ChangeSceneToPacked(nextScene);
    }

    public ulong GetElapsedMS()
    {
        return Time.GetTicksMsec() - tickStartTime;
    }
}
