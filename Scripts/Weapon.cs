using Godot;
using System;

public partial class Weapon : Area3D
{
    [Export]private int dmg = 1;

    public void OnBodyEntered(Node3D node)
    {
        if(node is Creature)
        {
            Creature c = (Creature)node;
            c.ChangeHP(-dmg, DamageSource.Bonk);
        }
    }
}
