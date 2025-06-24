using Godot;
using MagicSystem;
using System;

public partial class SpellHUD : Control
{
    [Export] Control[] spellUIPositions;
    [Export] TextureRect[] spellCircles;
    [Export] SpellMenu menu;

    public override void _Ready()
    {
        if (menu != null)
        {
            menu.SelectSpell += SpellSelectedInUI;
        }

        spellCircles[0] = GetNode<TextureRect>("XButtonThumbnail");
        spellCircles[1] = GetNode<TextureRect>("YButtonThumbnail");
        spellCircles[2] = GetNode<TextureRect>("BButtonThumbnail");

        SpellManager.SpellChange += UpdateCircle;
    }

    public override void _Process(double delta)
    {
        for (int i = 0; i < spellCircles.Length; i++)
        {
            if (spellCircles[i].GlobalPosition != spellUIPositions[i].GlobalPosition)
            {
                // swous
                spellCircles[i].GlobalPosition = KMath.MoveTowardParabolic(spellCircles[i].GlobalPosition, spellUIPositions[i].GlobalPosition, 100, (float)delta);
            }
        }
    }

    /// <summary>
    /// Should be called by SpellMenu.SelectSpell...
    /// Set the position of the UI circle to the selection box thumbnail thing.
    /// The Process of SpellHUD will then move it back to its proper place.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="name"></param>
    /// <param name="UIPos"></param>
    public void SpellSelectedInUI(int slot, MagicSystem.SpellManager.SpellName name, Vector2 UIPos)
    {
        spellCircles[slot].GlobalPosition = UIPos;
    }

    /// <summary>
    /// Update the Thumbnail for the specific spell.
    /// Automatically called when equipped spell for slot is changed.
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="name"></param>
    public void UpdateCircle(object sender, SpellManager.SpellChangeEventArgs e)
    {
        if (e.name != SpellManager.SpellName.None)
            GD.Print("thumbnail " + e.slot + ": " + (SpellManager.GetSpellData(e.name).thumbnail != null));
        else
        {
            GD.Print("slot " + e.slot + " is none");
        }

        // GD.Print(spellCircles[e.slot].Name);

        // spellCircles[e.slot].Texture = null;

        // spellCircles[e.slot].Texture = SpellManager.GetSpellData(e.name).thumbnail;
        
        (spellCircles[e.slot].Material as ShaderMaterial).SetShaderParameter("Thumbnail", SpellManager.GetSpellData(e.name).thumbnail);
    }
}
