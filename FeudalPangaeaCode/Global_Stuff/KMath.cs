using Godot;
using System;
using System.Numerics;
using Vector3 = Godot.Vector3;
using Vector2 = Godot.Vector2;

public static class KMath
{
    public static float Lerp(float firstFloat, float secondFloat, float by)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }

    public static float Dist3D(Vector3 first, Vector3 second)
    {
        return (float)Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2) + Math.Pow(first.Z - second.Z, 2));
    }

    public static float Dist2D(Vector2 first, Vector2 second)
    {
        return (float)Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
    }

    /// <summary>
    /// Returns the next step in moving towards the target position.
    /// It uses some weird parabolic curve that looks cool.
    /// This one is 2D btw.
    /// </summary>
    /// <param name="current">Current position</param>
    /// <param name="target">Target position</param>
    /// <param name="speed">speed multiplier</param>
    /// <param name="delta">delta time</param>
    /// <returns>Next step</returns>
    public static Vector2 MoveTowardParabolic(Vector2 current, Vector2 target, float speed, double delta, float minSpeed = 0)
    {
        float distance = KMath.Dist2D(current, target);

        Vector2 direction = (target - current).Normalized() * ParabolicSpeed(distance, minSpeed) * speed * (float)delta;

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

    /// <summary>
    /// Returns the next step in moving towards the target position.
    /// It uses some weird parabolic curve that looks cool.
    /// This one is 3D btw.
    /// </summary>
    /// <param name="current">Current position</param>
    /// <param name="target">Target position</param>
    /// <param name="speed">speed multiplier</param>
    /// <param name="delta">delta time</param>
    /// <returns>Next step</returns>
    public static Vector3 MoveTowardParabolic3D(Vector3 current, Vector3 target, float speed, double delta, float minSpeed = 0)
    {
        float distance = KMath.Dist3D(current, target);

        Vector3 direction = (target - current).Normalized() * ParabolicSpeed(distance, minSpeed) * speed * (float)delta;

        Vector3 step = current + direction;

        Vector3 dp1 = target - current;
        Vector3 dp2 = target - step;

        float dot = dp1.Dot(dp2);

        if (dot < 0)
        {
            return target;
        }

        return step;
    }

    /// <summary>
    /// Returns the next step in moving towards the target position.
    /// Straight shooting linear progression.
    /// This one is 3D btw.
    /// </summary>
    /// <param name="current">Current position</param>
    /// <param name="target">Target position</param>
    /// <param name="speed">speed multiplier</param>
    /// <param name="delta">delta time</param>
    /// <returns>Next step</returns>
    public static Vector3 MoveTowardLinear3D(Vector3 current, Vector3 target, float speed, double delta)
    {
        Vector3 direction = (target - current).Normalized() * speed * (float)delta;

        Vector3 step = current + direction;

        Vector3 dp1 = target - current;
        Vector3 dp2 = target - step;

        float dot = dp1.Dot(dp2);

        if (dot < 0)
        {
            return target;
        }

        return step;
    }

    /// <summary>
    /// MATHEMATICAL!
    /// Used to calculate the speed of the selector cursor.
    /// </summary>
    /// <param name="dist">Distance from the target position.</param>
    /// <returns></returns>
    public static float ParabolicSpeed(float dist, float minSpeed = 0)
    {
        float dMin = 150;

        float d = Mathf.Max(dMin, dist + 100);

        float speed = dist * (-dist + d) / d * 0.25f;

        return Math.Max(speed, minSpeed);
    }

    public static double RotateTowards(double current, double target, float speed, double delta)
    {
        //current and target will be between 0 - PI * 2
        double diff = target % (Math.PI * 2) - current % (Math.PI * 2);

        if (diff > Math.PI)
        {
            diff -= Math.PI * 2;
        }

        if (diff <= -Math.PI)
        {
            diff += Math.PI * 2;
        }

        if (diff == 0)
        {
            return target;
        }

        //now diff should be between -Math.PI - Math.PI

        int direction = diff > 0 ? 1 : -1;

        double next = current + direction * speed * delta;

        double change1 = next - target;
        double change2 = current - target;

        if (change1 * change2 < 0)
        {
            return target;
        }

        return next;
    }
}
