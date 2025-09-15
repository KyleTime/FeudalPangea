using Godot;
using System;
using System.Threading.Tasks;

public partial class RegenAnchor : Anchor
{
    [Export] public float respawnTime = 10;
    private float timer = 0;
    private bool cooldowning = false;

    bool initialWait = false;

    public override void _Process(double delta)
    {
        if (inProgress)
        {
            timer = 0;
            cooldowning = false;
            return;
        }

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
                cooldowning = false;
                ResetEntity();
            }
    }

}
