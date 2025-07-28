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

                if (Name == "FloatingWoodenBlockAnchor")
                {
                    GD.Print("Started Respawn Timer");
                }
            }
            else if (cooldowning && timer > 0)
            {
                if (timer == respawnTime && Name == "FloatingWoodenBlockAnchor")
                {
                    GD.Print("Timer has begun!");
                }

                timer -= (float)delta;
            }
            else if (cooldowning)
            {
                timer = 0;
                cooldowning = false;
                ResetEntity();

                if (Name == "FloatingWoodenBlockAnchor")
                    GD.Print("Reset Entity");
            }
    }

}
