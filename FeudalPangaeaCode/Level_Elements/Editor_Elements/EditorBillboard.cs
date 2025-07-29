using Godot;
using System;

public partial class EditorBillboard : Sprite3D
{
    public override void _Ready()
    {
        QueueFree();
    }
}
