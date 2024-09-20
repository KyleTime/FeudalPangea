using Godot;
using System;

public static class KMath
{
    public static float Lerp(float firstFloat, float secondFloat, float by)
    {  
        return firstFloat * (1 - by) + secondFloat * by;
    }
}
