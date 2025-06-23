using Godot;
using MagicSystem;
using System;
using System.Threading.Tasks;

public partial class SpellMenu : Control
{
    public static SpellMenu menu;

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

    [Signal]
    public delegate void SelectSpellEventHandler(int slot, SpellManager.SpellName spell, Vector2 UIPosition);

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

        UpdateSpellUI();

        Visible = false;

        menu = this;

        VisibilityChanged += ResetOnVisibilityChange;
    }

    public static SpellMenu GetMenu()
    {
        if (menu == null)
        {
            GD.PrintErr("SPELL MENU DOES NOT EXIST!");
            return null;
        }

        return menu;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible)
        {
            return;
        }

        MoveSelectorByInput();

        int spellButton = SpellManager.GetSpellButton();

        if (spellButton != -1)
            SelectSpellForSlot(spellButton);

        if (Input.IsActionJustPressed("QUIT"))
        {
            if (GetTree().Paused)
                TimeManager.PauseTime(false);

            Visible = false;
        }
    }

    /// <summary>
    /// Reads in user input to move the selector cursor around the menu.
    /// Also updates the Spell UI when the cursor moves.
    /// </summary>
    public void MoveSelectorByInput()
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

        if (leftRight != 0 || upDown != 0)
        {
            selection = Mathf.Clamp(selection + leftRight + upDown * gridXSize, 0, spellContainers.Length - 1);
            UpdateSpellUI();
        }
    }

    /// <summary>
    /// Updates the actual equipped spell in SpellManager, then
    /// emits the SelectSpell signal (which largely just tells the SpellHUD to update)
    /// </summary>
    /// <param name="slot"></param>
    public void SelectSpellForSlot(int slot)
    {
        SpellManager.SelectSpell(slot, spellContainers[selection].spellName);
        EmitSignal(SignalName.SelectSpell, slot, (int)spellContainers[selection].spellName, spellContainers[selection].GlobalPosition);
    }

    public override void _Process(double delta)
    {
        //this just updates where the selector cursor thing is going and moves it towards there
        nextSelectorPosition = GetSelectorPosition();
        selector.GlobalPosition = KMath.MoveTowardParabolic(selector.GlobalPosition, nextSelectorPosition, 100, delta);
    }

    /// <summary>
    /// Reads in all the proper data from the current spell container and updates the UI info.
    /// </summary>
    private void UpdateSpellUI()
    {
        title.Text = spellContainers[selection].name;
        desc.Text = spellContainers[selection].description;
        StyleBoxTexture tex = new StyleBoxTexture();
        tex.Texture = spellContainers[selection].pageImage;
        image.AddThemeStyleboxOverride("panel", tex);
    }

    /// <summary>
    /// Calculate where the selector cursor should go next.
    /// It's just the global position of the current spell container
    /// with a little bit of an offset.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetSelectorPosition()
    {
        return spellContainers[selection].GlobalPosition - selector.PivotOffset;
    }

    public void UpdateMenuByInventory()
    {
        for (int i = 0; i < SpellManager.spellInventory.Count && i < spellContainers.Length; i++)
        {
            SpellContainer c = spellContainers[i];
            SpellManager.SpellName n = SpellManager.spellInventory[i];
            SpellManager.SpellData data = SpellManager.GetSpellData(n);

            c.ReadInSpellData(n, data);
        }
    }

    public void ResetOnVisibilityChange()
    {
        if (Visible)
        {
            selection = 0;
            selector.GlobalPosition = GetSelectorPosition();
            UpdateMenuByInventory();
            UpdateSpellUI();
        }
    }
}
