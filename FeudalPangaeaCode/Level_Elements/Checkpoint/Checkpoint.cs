using Godot;
using System;
using System.Threading.Tasks;

public partial class Checkpoint : Node3D, Interactable
{
    [Export] public MeshInstance3D visual;

    public void Interact()
    {
        if (GetTree().Paused)
            return;

        TimeManager.PauseTime(true);

        SpellMenu.menu.Visible = true;
    }

    public async override void _Process(double delta)
    {
        if (InRangeOfPlayer() && !Player.player.interactables.Contains(this))
        {
            Player.player.interactables.Add(this);
        }

        await Task.Delay(100);
    }

    public bool InRangeOfPlayer()
    {
        return KMath.Dist3D(Player.player.GlobalPosition, GlobalPosition) < 5;
    }

    public new Vector3 GetPosition()
    {
        return GlobalPosition;
    }

    public void Highlighted(bool highlighted)
    {
        var meshMaterial = visual.Mesh.SurfaceGetMaterial(0);
        if (meshMaterial is StandardMaterial3D stdMaterial)
        {
            if (highlighted)
                stdMaterial.AlbedoColor = new Color(1, 0, 0);
            else
                stdMaterial.AlbedoColor = new Color(1, 1, 1);
        }
    }
}
