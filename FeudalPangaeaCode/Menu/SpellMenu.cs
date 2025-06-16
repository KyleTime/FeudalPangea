using Godot;
using System;
using System.Threading.Tasks;

public partial class SpellMenu : Control
{
    SpellContainer[] spellContainers;
    int gridXSize;
    int gridYSize;
    Label title;
    Label desc;
    Panel image;

    private int selection = 0;
    private Vector2 nextSelectorPosition;
    Control selector;
    bool selectorMoving = false;

    public override void _Ready()
    {
        title = GetNode<Label>("Title");
        desc = GetNode<Label>("Description");
        image = GetNode<Panel>("Spell_Image");
        selector = GetNode<Control>("Selector");

        GridContainer gridContainer = GetNode<GridContainer>("Spell_Containers");

        Godot.Collections.Array<Node> gridChildren = gridContainer.GetChildren();

        gridXSize = gridContainer.Columns;
        gridYSize = (gridChildren.Count + (gridChildren.Count % gridXSize)) / gridXSize;

        spellContainers = new SpellContainer[gridChildren.Count];

        int index = 0;
        for (int i = 0; i < gridChildren.Count; i++)
        {
            if (gridChildren[i] is SpellContainer)
            {
                spellContainers[index] = (SpellContainer)gridChildren[i];
                index++;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        float yAxis = 0;
        float xAxis = 0;

        int upDown = 0;
        int leftRight = 0;

        if (Input.IsActionJustPressed("FORWARD") || Input.IsActionJustPressed("BACKWARD"))
        {
            yAxis = Input.GetAxis("FORWARD", "BACKWARD");
            upDown = yAxis > 0 ? 1 : -1;

            if (upDown > 0 && spellContainers.Length - selection - 1 < gridXSize || upDown < 0 && selection < gridXSize)
            {
                upDown = 0;
            }
        }
        if (Input.IsActionJustPressed("LEFT") || Input.IsActionJustPressed("RIGHT"))
        {
            xAxis = Input.GetAxis("LEFT", "RIGHT");
            leftRight = xAxis > 0 ? 1 : -1;
        }

        selection = Mathf.Clamp(selection + leftRight + upDown * gridXSize, 0, spellContainers.Length - 1);
        GD.Print("selection: " + selection);
    }

    public override void _Process(double delta)
    {
        nextSelectorPosition = GetSelectorPosition();
        selector.GlobalPosition = MoveToward(selector.GlobalPosition, nextSelectorPosition, 100, delta);
    }

    public Vector2 GetSelectorPosition()
    {
        return spellContainers[selection].GlobalPosition - selector.PivotOffset;
    }

    public Vector2 MoveToward(Vector2 current, Vector2 target, float speed, double delta)
    {
        float distance = KMath.Dist2D(current, target);

        Vector2 direction = (target - current).Normalized() * ParabolicSpeed(distance) * speed * (float)delta;

        Vector2 step = current + direction;

        Vector2 dp1 = target - current;
        Vector2 dp2 = target - step;

        float dot = dp1.X * dp2.X + dp1.Y * dp2.Y;

        if (dot < 0)
        {
            return target;
        }

        return step;
    }

    public float ParabolicSpeed(float dist)
    {
        float dMin = 150;

        float d = Mathf.Max(dMin, dist + 100);

        return ((dist * (-dist + d)) / d) * 0.25f;
    }
}
