using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Dialogue Manager (DM)
/// </summary>
public static class DM
{   
    // Player
    public static HUDHandler hud;
    public static int activeSelection = 0; //maybe give to hud
    
    public static DialogueTree tree = null;
    

    public static void Enter(Node talker, DialogueTree dialogue)
    {
        tree = dialogue;
        tree.talker = talker;
        hud.EnterDialogue(tree.activeFrame);
    }

    public static void incSelect()
    {
        if(tree==null)return;
        activeSelection = (activeSelection + 1)%tree.activeFrame.numSelections;
    } //probably a better way to do this
    public static void decSelect() //^
    {
        if(tree==null)return;
        activeSelection = (activeSelection - 1)%tree.activeFrame.numSelections;
    }

    public static DialogueFrame Advance()
    {
        if(tree.AdvanceFrame(activeSelection) == null)
        {
            return null;
        }
        hud.UpdateDialogue(tree.activeFrame);//update hud
        return tree.activeFrame;
    }

    public static void Exit()
    {
        tree = null;
        activeSelection = 0;
        hud.ExitDialogue();
    }

    public static bool IsActive()
    {
        return (tree != null);
    }
}


public enum FrameBehavior {Start, Home, Target, Escape}

public class DialogueTree
{       
    Dictionary<string, DialogueFrame> frames;
    public DialogueFrame activeFrame;
    string filepath;
    DialogueFrame home;
    public Node talker;

    public DialogueTree()
    {
        frames = new Dictionary<string, DialogueFrame>();
        AddBlankFrame("MT");
        InitActive();
    }

    public DialogueTree(string text)
    {
        frames = new Dictionary<string, DialogueFrame>();
        frames.Add("0", new DialogueFrame("0", text));
        InitActive();
    }

    public DialogueTree(DialogueFrame singleFrame) : this()
    {
        frames = new Dictionary<string, DialogueFrame>();
        frames.Add(singleFrame.frameKey, singleFrame);
        InitActive();
    }

    public DialogueTree(DialogueFrame[] arrayOfFrames)
    {
        frames = new Dictionary<string, DialogueFrame>();
        foreach (DialogueFrame f in arrayOfFrames){
            frames.Add(Array.IndexOf(arrayOfFrames, f).ToString(), f);
        }
        InitActive();
    }
    
    void InitActive() //replace name probably
    {
        activeFrame = frames.Values.ElementAt(0); //index 0 is always the start
        home = frames.Values.ElementAt(0); //default "home" frame is index 0
    }
    
    public void AddBlankFrame(string frameKey){frames.Add(frameKey, new DialogueFrame(frameKey));}//FrameBehavior.Escape

    public DialogueFrame AdvanceFrame(int activeSelection)
    {
        switch (activeFrame.behavior){
            case FrameBehavior.Start:
                activeFrame = frames.Values.ElementAt(0);
                break;
            case FrameBehavior.Home:
                activeFrame = home;
                break;
            case FrameBehavior.Target:
                activeFrame = frames[activeFrame.GetNextFrame(activeSelection)];
                break;
            case FrameBehavior.Escape:
                DM.Exit();
                return null;
                break;
        }

        DM.activeSelection = 0;
        return activeFrame;
    }
}

public class DialogueFrame
{
    string body;
    public string frameKey {get;}
    public int numSelections {get;}
    string nextFrame; //key
    public FrameBehavior behavior {get;}

    public DialogueFrame(string key)
    {
        body = "empty";
        frameKey = key;
        numSelections = 0;
        behavior = FrameBehavior.Escape;
    }
    //Now with text!!
    public DialogueFrame(string key, string text)
    {
        body = text;
        frameKey = key;
        numSelections = 0;
        behavior = FrameBehavior.Escape;
    }

    public string getText()
    {
        return body;
    }
    public string GetNextFrame(int activeSelection)
    {
        return nextFrame;
    }

}