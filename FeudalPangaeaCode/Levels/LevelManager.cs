using Godot;
using MagicSystem;
using System;

public partial class LevelManager : Node
{
    public static LevelManager currentLevel;

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
        GetTree().ReloadCurrentScene();
    }
}
