using Godot;
using System;

public partial class Thumbnail : Node
{
    [Export] Texture2D image;
    [Export] string name;
    [Export] PackedScene levelScene;

    TextureRect rect;
    Label label;
    Button play;

    public override void _Ready()
    {
        rect = GetNode<TextureRect>("Panel/TextureRect");
        label = GetNode<Label>("Label");
        play = GetNode<Button>("Button");

        rect.Texture = image;
        label.Text = name;
        play.ButtonDown += LoadLevel;
    }

    public void LoadLevel()
    {
        GetTree().ChangeSceneToPacked(levelScene);
    }

}
