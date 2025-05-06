using Godot;
using System;

public partial class DeathPlane : Area3D
{
    public void OnBodyEntered(Node3D body){
        if(body is ICreature)
        {
            ICreature c = (ICreature)body;
            c.ChangeHP(-99999999, DamageSource.Fall);
        }
    }
}
