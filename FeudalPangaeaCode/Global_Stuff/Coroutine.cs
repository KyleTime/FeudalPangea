using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public static class Coroutine 
{
    public static async void StartCoroutine(IEnumerable objects)       
    {
        var mainLoopTree = Engine.GetMainLoop();
        foreach (var time in objects)
        {   
            if(time is int){
                await Task.Delay((int)time);
                continue;
            }

            await mainLoopTree.ToSignal(mainLoopTree, SceneTree.SignalName.ProcessFrame);
        }
    }
}