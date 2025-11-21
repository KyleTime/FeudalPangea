using Godot;
using System;
using System.Threading.Tasks;

public partial class wobbegong : Node3D, Interactable
{	
	DialogueTree wobbegongSpeaks = new DialogueTree("Hello I am Wobbegong (fish) and I would like to speak to you.");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public async override void _Process(double delta)
    {
        if (InRangeOfPlayer() && !Player.player.interactables.Contains(this))
        {
            Player.player.interactables.Add(this);
        }

        await Task.Delay(100);
    }

	public override void _PhysicsProcess(double delta)
	{
		LookAt(Player.player.cam.cam.GlobalPosition);
	}

	public void Interact()
	{
		DM.Enter(this, wobbegongSpeaks);
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
		if(highlighted)
		{
			return;
		}
		if(!highlighted)
		{
			DM.Exit();
		}
	}
}