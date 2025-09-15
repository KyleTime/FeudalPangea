using Godot;
using System;
using System.Collections.Generic;

public partial class Options : Node
{
    [Export] OptionButton resOptionButton;
    [Export] CheckBox fullscreenCheckBox;
    [Export] CheckBox vsyncCheckBox;
    [Export] OptionButton screens;

    Dictionary<string, Vector2I> Resolutions = new Dictionary<string, Vector2I>() {
                                {"3840x2160", new Vector2I(3840, 2160)},
                                {"2560x1440", new Vector2I(2560, 1080)},
                                {"1920x1080", new Vector2I(1920, 1080)},
                                {"1366x768", new Vector2I(1366, 768)},
                                {"1536x864", new Vector2I(1536, 864)},
                                {"1280x720", new Vector2I(1280, 720)},
                                {"1440x900", new Vector2I(1440, 900)},
                                {"1600x900", new Vector2I(1600, 900)},
                                {"1024x600", new Vector2I(1024, 600)},
                                {"800x600", new Vector2I(640, 480)}
                        };

    public override void _Ready()
    {
        CheckVariables();
        AddResolutions();
        GetScreens();

        resOptionButton.ItemSelected += ResolutionSelected;
        fullscreenCheckBox.Toggled += ToggleFullScreen;
        vsyncCheckBox.Toggled += ToggleVSync;
    }

    public void CheckVariables()
    {
        Window _window = GetWindow();
        Window.ModeEnum mode = _window.GetMode();

        if (mode == Window.ModeEnum.Fullscreen)
        {
            resOptionButton.Disabled = true;
            fullscreenCheckBox.SetPressedNoSignal(true);
        }

        if (DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled)
        {
            vsyncCheckBox.SetPressedNoSignal(true);
        }
    }

    public void AddResolutions()
    {
        Vector2I resolution = GetWindow().Size;
        int ID = 0;

        foreach (var pair in Resolutions)
        {
            resOptionButton.AddItem(pair.Key, ID);

            if (pair.Value == resolution)
                resOptionButton.Select(ID);

            ID++;
        }
    }

    public void ToggleFullScreen(bool toggledOn)
    {
        resOptionButton.Disabled = toggledOn;

        if (toggledOn)
        {
            GetWindow().SetMode(Window.ModeEnum.Fullscreen);
        }
        else
        {
            GetWindow().SetMode(Window.ModeEnum.Windowed);
            CenterWindow();
        }
    }

    public void CenterWindow()
    {
        var CenterScreen = DisplayServer.ScreenGetPosition() + DisplayServer.ScreenGetSize() / 2;
        var WindowSize = GetWindow().GetSizeWithDecorations();
        GetWindow().SetPosition(CenterScreen - WindowSize / 2);
    }

    public void ResolutionSelected(long index)
    {
        var ID = resOptionButton.GetItemText((int)index);
        GetWindow().SetSize(Resolutions[ID]);
        CenterWindow();
    }

    public void ToggleVSync(bool toggledOn)
    {
        if (toggledOn)
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
        }
        else
        {
            DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
        }
    }

    public void GetScreens()
    {
        int screenCount = DisplayServer.GetScreenCount();

        for (int i = 0; i < screenCount; i++)
        {
            screens.AddItem("Screen: " + i);
        }
    }

    public void OnScreenSelected(int index)
    {
        Window window = GetWindow();

        var mode = window.GetMode();

        window.SetMode(Window.ModeEnum.Windowed);
        window.SetCurrentScreen(index);

        if (mode == Window.ModeEnum.Fullscreen)
        {
            window.SetMode(Window.ModeEnum.Fullscreen);
        }
    }
}
