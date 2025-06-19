using Godot;
using System;
using System.Threading.Tasks;

public partial class HUDHandler : Node
{
    [Export] Texture2D[] hpFrames = new Texture2D[7];
    [Export] TextureRect rect;

    [Export] ColorRect diedGraphic;
    [Export] Panel diedPanel;

    [Export] RichTextLabel coinCounter;

    public override void _Process(double delta)
    {
        base._Process(delta);

        //this is terrible, please do an event somehow
        coinCounter.Text = GlobalData.GetCoinCount().ToString();
    }


    public void ChangeHPBar(int HP, int MAX_HP)
    {
        float percentage = (float)HP / MAX_HP;
        int hpDisplay = (int)(percentage * 6);

        GD.Print("HP: " + HP + " MAX_HP: " + MAX_HP);

        GD.Print("percentage: " + percentage + " hpDisplay: " + hpDisplay);

        rect.Texture = hpFrames[hpDisplay];
    }

    public async Task HUD_Death_Animation()
    {
        await Task.Delay(1000); //time before thing appears
        diedPanel.Scale = new Vector2(1, 1);
        diedGraphic.Color = new Color(diedGraphic.Color.R, diedGraphic.Color.B, diedGraphic.Color.G, 0);
        diedGraphic.Visible = true;

        Label label = diedGraphic.GetNode<Label>("Label");

        float first_time = 1.5f;
        float last_time = 1;

        float timer = 0;
        while (timer < first_time)
        {
            diedPanel.Scale = new Vector2(1 + timer * 0.2f, 1 + timer * 0.2f);
            diedGraphic.Color = new Color(diedGraphic.Color.R, diedGraphic.Color.B, diedGraphic.Color.G, Math.Clamp(timer, 0, 1));
            label.LabelSettings.FontColor = new Color(label.LabelSettings.FontColor.R, label.LabelSettings.FontColor.B, label.LabelSettings.FontColor.G, Math.Clamp(timer, 0, 1));
            timer += (float)GetProcessDeltaTime();
            await Task.Delay(1);
        }

        timer = 0;

        while (timer < last_time)
        {
            diedPanel.Scale = new Vector2(1 + (timer + first_time) * 0.2f, 1 + (timer + first_time) * 0.2f);
            diedGraphic.Color = new Color(diedGraphic.Color.R, diedGraphic.Color.B, diedGraphic.Color.G, Math.Clamp(1 - timer, 0, 1));
            label.LabelSettings.FontColor = new Color(label.LabelSettings.FontColor.R, label.LabelSettings.FontColor.B, label.LabelSettings.FontColor.G, Math.Clamp(1 - timer, 0, 1));
            timer += (float)GetProcessDeltaTime();
            await Task.Delay(1);
        }

        await Task.Delay(500);

        GD.Print("Routine end");
        diedGraphic.Visible = false;
    }
}
