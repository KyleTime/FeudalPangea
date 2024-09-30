using Godot;
using System;

public partial class DeathPlane : Area3D
{
    public void OnBodyEntered(Node3D body){
        if(body is Creature)
        {
            Creature c = (Creature)body;
            c.Death(DamageSource.Fall);
        }
    }
}
