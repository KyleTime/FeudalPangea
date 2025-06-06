using Godot;
using System;

public static class KMath
{
    public static float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    public static float Dist3D(Vector3 first, Vector3 second)
    {
        return (float)Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
    }
}
