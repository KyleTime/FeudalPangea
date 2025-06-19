using Godot;
using System;

public partial class SpellContainer : Control
{
    [Export] public string name;
    [Export] public string description;
    [Export] public Texture2D pageImage;
    [Export] public Texture2D thumbnail;
    [Export] public MagicSystem.SpellManager.SpellName spellName;

    public override void _Ready()
    {
        if (thumbnail != null)
        {
            GetNode<TextureRect>("TextureRect").Texture = thumbnail;
        }
    }
}
