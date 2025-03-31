using Godot;
using System;

public partial class Hitbox : Area3D
{
    [Export]private int dmg = 1;
    [Export]private float push_force = 10;
    [Export]private DamageSource damage_source = DamageSource.Bonk;

    public override void _Ready(){
        BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node3D node)
    {
        if(node is Creature)
        {
            Creature c = (Creature)node;
            c.ChangeHP(-dmg, damage_source);

            switch(damage_source)
            {
                case DamageSource.Bonk:
                    c.Push((GlobalPosition - node.GlobalPosition).Normalized() * -push_force);
                    break;
            }
        }
    }
}
