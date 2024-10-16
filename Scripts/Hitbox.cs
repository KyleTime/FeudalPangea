using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export]private int dmg = 1;
    [Export]private float push_force = 10;

    public void OnBodyEntered(Node3D node)
    {
        if(node is Creature)
        {
            Creature c = (Creature)node;
            c.ChangeHP(-dmg, DamageSource.Bonk);
            c.Push((Position - node.Position).Normalized() * -push_force);
        }
    }
}
