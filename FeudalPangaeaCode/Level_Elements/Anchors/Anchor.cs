using Godot;
using System;
using System.Threading.Tasks;

public partial class Anchor : Node3D
{
    [Export] PackedScene entity;
    [Export] Vector3 size = new Vector3(1, 1, 1);
    [Export] Vector3 rotation = new Vector3(0, 0, 0);
    protected Node3D current;

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
            ent.GlobalRotation = rotation;
            ent.Scale = size;
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
