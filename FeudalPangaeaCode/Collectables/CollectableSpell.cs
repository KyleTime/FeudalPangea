using Godot;
using MagicSystem;
using System;

public partial class CollectableSpell : Collectable
{
    [Export] SpellManager.SpellName spell;
    [Export] public double collectRadius = 2;
    [Export] Decal decal;

    public override void _Ready()
    {
        base.radius = collectRadius;
        decal.TextureAlbedo = SpellManager.GetSpellData(spell).thumbnail;
    }

    public override void _EnterTree()
    {
        if (SpellManager.spellInventory.Contains(spell))
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        RotateY((float)(delta * Math.PI * 1));
    }


    public override void Collect()
    {
        SpellManager.spellInventory.Add(spell);

        QueueFree();
    }
}
