using Godot;
using System;

public partial class TitleScreen : Control
{
    [Export] Button play;
    [Export] Button quit;
    [Export] Control levelMenu;

    public override void _Ready()
    {
        Visible = true;
        levelMenu.Visible = false;

        play.ButtonDown += SwitchToLevelMenu;
        quit.ButtonDown += QuitGame;
    }

    private void SwitchToLevelMenu()
    {
        Visible = false;
        levelMenu.Visible = true;
    }

    private void QuitGame()
    {
        GetTree().Quit();
    }
}
