using Godot;
using System;
using System.Threading.Tasks;

public partial class Anchor : Node3D
{
    [Export] PackedScene entity;
    Node3D current;

    public async override void _Ready()
    {
        CreateEntity();

        //wait just a lil
        await Task.Delay(1);

        LevelManager.currentLevel.ResetLevel += ResetEntity;
    }

    protected virtual void ResetEntity()
    {
        if (IsInstanceValid(current))
            current.QueueFree();
        CreateEntity();
    }

    protected async virtual void CreateEntity()
    {
        Node3D ent = (Node3D)entity.Instantiate();

        Node root = GetTree().Root;
        root.CallDeferred(Node.MethodName.AddChild, ent);

        await Task.Delay(100);

        if (IsInstanceValid(ent))
        {
            ent.GlobalPosition = GlobalPosition;
            current = ent;
        }
        else
        {
            //if our instance disappeared, try to remake it in 1 second
            await Task.Delay(1000);
            CreateEntity();
        }
    }
}
