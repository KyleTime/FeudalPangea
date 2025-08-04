using Godot;
using System;
using System.Threading.Tasks;

public partial class Checkpoint : Node3D, Interactable
{
    [Export] public MeshInstance3D visual;
    [Export] public HitFx checkpointEffect;
    [Export] public Vector3 checkpointPosOffset = new Vector3();
    [Export] public Vector3 checkpointRot = new Vector3();

    static Checkpoint current = null;

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

            if (current != this)
            {
                if (checkpointEffect != null)
                {
                    checkpointEffect.Effect(GlobalPosition, GlobalRotation);
                }

                Player.player.checkpointPosition = GlobalPosition + checkpointPosOffset;
                Player.player.checkpointRotation = checkpointRot;

                current = this;
            }
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
