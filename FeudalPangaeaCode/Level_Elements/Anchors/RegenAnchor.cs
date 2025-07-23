using Godot;
using System;
using System.Threading.Tasks;

public partial class RegenAnchor : Anchor
{
    [Export] public float respawnTime = 10;
    private float timer = 0;
    private bool cooldowning = false;

    public async override void _Process(double delta)
    {
        if (!cooldowning && !IsInstanceValid(current) && timer == 0)
        {
            timer = respawnTime;
            cooldowning = true;
        }
        else if (cooldowning && timer > 0)
        {
            timer -= (float)delta;
        }
        else if (cooldowning)
        {
            timer = 0;
            ResetEntity();
            cooldowning = false;
            await Task.Delay(2000); //this is probably optimal because I'd imagine the thing won't die again for at least 2 seconds
        }
    }

}
