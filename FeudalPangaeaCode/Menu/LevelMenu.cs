using Godot;
using System;

public partial class LevelMenu : Control
{
    [Export] public Control previousMenu;
    [Export] public Button backButton;

    public override void _Ready()
    {
        backButton.ButtonDown += () => { previousMenu.Visible = true; Visible = false; };
    }

}
