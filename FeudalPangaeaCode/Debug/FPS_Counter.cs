using Godot;
using System;

public partial class FPS_Counter : RichTextLabel
{
    public override void _Process(double delta)
    {
        Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
    }

}
