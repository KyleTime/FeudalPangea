using Godot;
using System;
using System.Threading.Tasks;

public partial class FallingObject : Node3D
{
    enum PlatformType
    {
        Falling, //moves the distance in the direction and then disappears, respawns after resetDelay
        Rebound, //moves the distance and changes direction and moves back before resetting, hangs at each end for resetDelay seconds
        Hang //moves the distance in the direction and hangs at the end for resetDelay before resetting 
    }

    enum GraphType
    {
        Linear,
        Parabolic
    }

    [Export] PlatformType platformType;
    [Export] GraphType graphType;

    [Export] float speed = 1;
    [Export] float distance = 5;
    [Export] Vector3 fallDirection = new Vector3(0, -1, 0);
    [Export] float resetDelay = 0;
    [Export] float startDelay = 0;
    float timer = 0;
    Vector3 startPosition;
    Vector3 endPosition;
    bool targetEnd = true;

    public async override void _EnterTree()
    {
        startPosition = GlobalPosition;
        endPosition = startPosition + fallDirection * distance;

        while (!IsInstanceValid(LevelManager.currentLevel))
        {
            await Task.Delay(100);
        }

        LevelManager.currentLevel.ResetLevel += ResetSelf;
    }

    public async override void _PhysicsProcess(double delta)
    {
        if (timer < startDelay)
        {
            timer += (float)delta;
            return;
        }

        //move platform towards the target
        switch (graphType)
        {
            case GraphType.Linear:
                GlobalPosition = KMath.MoveTowardLinear3D(GlobalPosition, targetEnd ? endPosition : startPosition, speed, delta);
                break;
            case GraphType.Parabolic:
                GlobalPosition = KMath.MoveTowardParabolic3D(GlobalPosition, targetEnd ? endPosition : startPosition, speed, delta, 0.5f);
                break;
        }

        switch (platformType)
        {
            case PlatformType.Falling: //assume we're always targetting endPosition
                if (GlobalPosition == endPosition)
                {
                    ProcessMode = ProcessModeEnum.Disabled; //disable self for a lil
                    Visible = false;

                    await Task.Delay((int)(resetDelay * 1000));

                    if (!IsInstanceValid(this) || Visible)
                    {
                        return;
                    }

                    ResetSelf();
                }

                break;
            case PlatformType.Rebound:

                if (targetEnd && GlobalPosition == endPosition)
                {
                    await Task.Delay((int)(resetDelay * 1000));

                    if (!IsInstanceValid(this))
                    {
                        return;
                    }

                    targetEnd = false;
                }
                else if (!targetEnd && GlobalPosition == startPosition)
                {
                    await Task.Delay((int)(resetDelay * 1000));

                    if (!IsInstanceValid(this))
                    {
                        return;
                    }
                    
                    targetEnd = true;
                }

                break;
            case PlatformType.Hang:

                if (GlobalPosition == endPosition)
                {
                    await Task.Delay((int)(resetDelay * 1000));

                    if (!IsInstanceValid(this))
                    {
                        return;
                    }

                    ProcessMode = ProcessModeEnum.Disabled; //disable self for a lil
                    Visible = false;

                    ResetSelf();
                }

                break;
        }
    }

    /// <summary>
    /// The reason this has to be so weird is because of issues with AnimatableBody3D
    /// This setup fucking sucks but I have no idea what else to do
    /// </summary>
    public async void ResetSelf()
    {
        timer = 0;

        SetDeferred(Node3D.PropertyName.GlobalPosition, startPosition);
        targetEnd = true;

        while (GlobalPosition != startPosition)
        {
            await Task.Delay(500);
        }

        SetDeferred(Node.PropertyName.ProcessMode, 0);
        SetDeferred(Node3D.PropertyName.Visible, true);
    }
}
