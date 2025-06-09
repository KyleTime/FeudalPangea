using Godot;
using System;
using System.Threading.Tasks;

public partial class PauseMenu : Control
{
    private bool paused = false;

    [Export] public Button cont;
    [Export] public Button quit;

    public override void _Ready()
    {
        cont.ButtonDown += ContinueButton;
        quit.ButtonDown += QuitButton;
    }

    public void QuitButton()
    {
        GetTree().Quit();
    }

    public void ContinueButton()
    {
        UnPause();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("QUIT"))
        {
            if (paused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        //Visible = true;
        TimeManager.PauseTime(true);
        paused = true;
    }

    public async void UnPause()
    {
        await Task.Delay(10);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Visible = false;
        TimeManager.PauseTime(false);
        paused = false;
    }
}
