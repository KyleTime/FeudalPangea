using Godot;
using System;

public partial class EditorBillboard : Node
{
    public override void _Ready()
    {
        QueueFree();
    }
}
