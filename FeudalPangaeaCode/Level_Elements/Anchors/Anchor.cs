using Godot;
using System;
using System.Threading.Tasks;

public partial class Anchor : Node3D
{
    [Export] PackedScene entity;
    [Export] Vector3 size = new Vector3(1, 1, 1);
    [Export] Vector3 rot = new Vector3(0, 0, 0);
    [Export] bool useAnchorNodeStatsInstead = false;
    /// <summary>
    /// Override the default behavior of adding the new object to the tree and instead add it to a specific node
    /// </summary>
    [Export] protected Node3D rootOverride;
    protected Node3D current;
    
    /// <summary>
    /// This is true when the anchor is currently creating or resetting an entity
    /// </summary>
    protected bool inProgress = false;

    public async override void _Ready()
    {
        if (useAnchorNodeStatsInstead)
        {
            size = Scale;
            rot = GlobalRotation;
        }

        await CreateEntity();

        while (LevelManager.currentLevel == null)
        {
            await Task.Delay(100);
        }

        LevelManager.currentLevel.ResetLevel += ResetEntityOnLevelReset;
    }

    /// <summary>
    /// Passes the ResetLevel event to the ResetEntity function.
    /// </summary>
    protected void ResetEntityOnLevelReset()
    {
        ResetEntity();
    }

    /// <summary>
    /// Resets the current entity. It's asynchronous to account for the time it takes the engine
    /// to figure out creating an entity so we can properly assign it a parent and position.
    /// It's really messy but as long as wherever this is called awaits it, it should be fine.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task ResetEntity()
    {
        while (inProgress)
        {
            await Task.Delay(10);
        }

        if (IsInstanceValid(current))
            current.QueueFree();
        await CreateEntity();
    }

    /// <summary>
    /// Creates a new entity to be kept track of by the anchor.
    /// Uses asynchronous behavior to allow the engine to figure out the object.
    /// MAKE SURE YOU AWAIT THIS OTHERWISE WEIRD THINGS MAY HAPPEN!
    /// </summary>
    /// <returns></returns>
    protected async virtual Task CreateEntity()
    {
        inProgress = true;

        Node3D ent = (Node3D)entity.Instantiate();

        //this little bit makes sure our new node doesn't do anything weird
        //before it officially exists
        ProcessModeEnum defMode = ent.ProcessMode;
        ent.ProcessMode = ProcessModeEnum.Disabled;
        ent.Visible = false;

        bool rootIsParent = rootOverride == null || !IsInstanceValid(rootOverride);

        ent.Scale = size;

        if (rootIsParent)
        {
            GetTree().Root.CallDeferred(Node.MethodName.AddChild, ent);
        }
        else
        {
            rootOverride.CallDeferred(Node.MethodName.AddChild, ent);
        }

        // await Task.Delay(1000);

        if (IsInstanceValid(ent))
        {
            current = ent;

            if (rootIsParent)
            {
                current.SetDeferred(Node3D.PropertyName.GlobalPosition, GlobalPosition);
            }
            else
            {
                current.SetDeferred(Node3D.PropertyName.Position, new Vector3());
            }

            current.SetDeferred(Node3D.PropertyName.GlobalRotation, rot);

            current.SetDeferred(Node3D.PropertyName.ProcessMode, (int)defMode);
            current.SetDeferred(Node3D.PropertyName.Visible, true);

            // current.SetDeferred(Node3D.PropertyName.Scale, size);
        }
        else
        {
            //if our instance disappeared, try to remake it in 1 second
            await Task.Delay(1000);
            await CreateEntity();
        }

        inProgress = false;
    }
}
