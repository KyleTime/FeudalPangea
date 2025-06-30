using Godot;
using MagicSystem;
using System;

public partial class SpellContainer : Control
{
    [Export] public string name;
    [Export] public string description;
    [Export] public Texture2D pageImage;
    [Export] public Texture2D thumbnail;
    [Export] public MagicSystem.SpellManager.SpellName spellName;

    public void ReadInSpellData(SpellManager.SpellName spell, SpellManager.SpellData data)
    {
        name = data.name;
        description = data.description;
        pageImage = data.pageImage;
        thumbnail = data.thumbnail;
        spellName = spell;

        GetNode<TextureRect>("TextureRect").Texture = thumbnail;
    }
}
