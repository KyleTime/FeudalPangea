using Godot;
using System;

//This is a surprise tool that will help us later... probably...
public partial class LevelManager : Node
{
    public override void _Ready()
    {
        base._Ready();
        ProcessMode = Node.ProcessModeEnum.Pausable;
    }

}
