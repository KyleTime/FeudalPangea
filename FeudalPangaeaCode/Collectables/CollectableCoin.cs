using Godot;
using System;

public partial class CollectableCoin : Collectable
{
    [Export] public double coinRadius = 2;

    public override void _Ready()
    {
        base.radius = coinRadius;
    }

    public override void _Process(double delta)
    {
        RotateY((float)(delta * Math.PI * 2));
    }


    public override void Collect()
    {
        GlobalData.ModCoinCount(1);

        QueueFree();
    }
}
