using Godot;
using System;

public partial class WinFlag : Node
{
    bool won = false;

    /// <summary>
    /// The area3D this references should ALWAYS only be looking for the player.
    /// Otherwise it'll do funky stuff.
    /// </summary>
    /// <param name="other"></param>
    public void BodyEntered(Node3D other)
    {
        if (won)
            return;

        LevelManager.currentLevel.Win(GlobalData.GetPackedScene("mainMenu"), GlobalData.standardDelayAfterWinMS);
        won = true;
    }
}
