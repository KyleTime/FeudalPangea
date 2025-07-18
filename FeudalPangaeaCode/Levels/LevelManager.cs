using Godot;
using MagicSystem;
using System;

public partial class LevelManager : Node
{
    public static LevelManager currentLevel;

    /// <summary>
    /// When the player spawns, where should they be?
    /// </summary>
    [Export] public Vector3 startPos = new Vector3();
    [Export] public Vector3 startRot = new Vector3();

    [Signal]
    public delegate void ResetLevelEventHandler();

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Pausable;
        currentLevel = this;

        if (!HasNode("FatherTime"))
        {
            Node time = GlobalData.GetPackedScene("fatherTime").Instantiate();
            AddChild(time);
        }
    }

    public void ReloadLevel()
    {
        TimeManager.PauseTime(false);
        EmitSignal(SignalName.ResetLevel);
    }
}
