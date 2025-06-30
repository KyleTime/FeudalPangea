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
    }

    public void ReloadLevel()
    {
        GetTree().ReloadCurrentScene();
    }
}
