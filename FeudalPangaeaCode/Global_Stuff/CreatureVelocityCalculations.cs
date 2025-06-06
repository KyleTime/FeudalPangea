using Godot;

//class to store general logic for movement that many other scripts may want
public static class CreatureVelocityCalculations
{
    /// <summary>
    /// Take in a node and velocity and rotate that body to face the direction it's moving.
    /// </summary>
    /// <param name="node">Node to rotate</param>
    /// <param name="velocity">Velocity to compare against</param>
    /// <param name="mod">modifier for the velocity, set to -1 if you want the object to face backwards</param>
    public static void RotateBody(Node3D node, Vector3 velocity, float mod = 1)
    {
        Vector3 flatVelocity = new Vector3(velocity.X, 0, velocity.Z) * mod;
        if (flatVelocity.Length() > 1)
        {
            node.LookAt(node.GlobalPosition - flatVelocity);

            node.Rotation = new Vector3(0, node.Rotation.Y, 0);
        }
    }

    /// <summary>
    /// Given the current velocity and a bunch of other parameters, spits back out an accelerated velocity.
    /// </summary>
    /// <param name="velocity">The current velocity of the object.</param>
    /// <param name="acceleration">The rate of acceleration (something like 20 is nice)</param>
    /// <param name="deceleration">The rate of deceleration (something like 10 works well)</param>
    /// <param name="maxSpeed">The maximum velocity (horizontally) the object can accelerate to.</param>
    /// <param name="XInput"></param>
    /// <param name="ZInput"></param>
    /// <param name="xDir"></param>
    /// <param name="zDir"></param>
    /// <param name="delta"></param>
    /// <param name="decelerate"></param>
    /// <returns></returns>
    public static Vector3 Accelerate(Vector3 velocity, float acceleration, float deceleration, float maxSpeed, float XInput, float ZInput, Vector3 xDir, Vector3 zDir, double delta, bool decelerate = true)
    {
        Vector3 nextVel = velocity;

        nextVel += XInput * xDir * acceleration * (float)delta;
        nextVel += ZInput * zDir * acceleration * (float)delta;

        float runningSpeed = new Vector3(velocity.X, 0, velocity.Z).Length();
        float nextRunningSpeed = new Vector3(nextVel.X, 0, nextVel.Z).Length();

        //GD.Print("Running Speed: " + runningSpeed + " Next Speed: " + nextRunningSpeed);

        //if we go over speed cap, cap it
        if (runningSpeed < maxSpeed + 0.1f && nextRunningSpeed > maxSpeed)
        {
            Vector3 runVector = new Vector3(nextVel.X, 0, nextVel.Z).Normalized() * maxSpeed;
            return new Vector3(runVector.X, velocity.Y, runVector.Z);
        }
        //if we're under the speed cap, or we're slowing down, accelerate as normal
        else if (nextRunningSpeed < maxSpeed || nextRunningSpeed <= runningSpeed)
        {
            return nextVel;
        }
        //we must be trying to go faster while already going too fast, decelerate instead
        else if (decelerate)
        {
            return Decelerate(velocity, deceleration, delta);
        }

        return velocity;
    }

    public static Vector3 Accelerate(Vector3 velocity, float acceleration, float deceleration, float maxSpeed, Vector3 direction, double delta, bool decelerate = true)
    {
        direction = direction.Normalized();
        return Accelerate(velocity, acceleration, deceleration, maxSpeed, 0, 1, new Vector3(), direction, delta, decelerate);
    }

    public static Vector3 Decelerate(Vector3 velocity, float deceleration, double delta)
    {
        Vector3 runVector = velocity;
        float yVel = velocity.Y;
        velocity.Y = 0;
        velocity -= velocity.Normalized() * deceleration * (float)delta;
        velocity.Y = yVel;

        if (!((runVector.X * velocity.X) > 0))
        {
            velocity.X = 0;
        }

        if (!((runVector.Z * velocity.Z) > 0))
        {
            velocity.Z = 0;
        }

        return velocity;
    }

    /// <summary>
    /// Takes in a Y velocity and deltatime and applies gravity.
    /// </summary>
    /// <param name="yVel">Current Y velocity</param>
    /// <param name="delta">Delta Time</param>
    /// <param name="gravityMod">Optional Gravity modifier</param>
    /// <returns>The new Y velocity</returns>
    public static float Gravity(float yVel, double delta, float gravityMod = 1)
    {
        yVel -= GlobalData.gravityRate * (float)delta * gravityMod;

        if (yVel < -GlobalData.maxGravity)
        {
            yVel = -GlobalData.maxGravity;
        }

        return yVel;
    }

    /// <summary>
    /// A function to easily calculate pushing something away.
    /// I am so tired of doing this manually.
    /// </summary>
    /// <param name="pusherPos">The position of the thing doing the pushing</param>
    /// <param name="pushedPos">The position of the thing being pushed</param>
    /// <param name="magnitude">Magnitude of the push</param>
    /// <returns>The calculated push vector</returns>
    public static Vector3 PushVector(Vector3 pusherPos, Vector3 pushedPos, float magnitude)
    {
        Vector3 direction = (pushedPos - pusherPos).Normalized();


        return direction * magnitude;
    }

        /// <summary>
    /// A function to push a creature away from another.
    /// Calculates the push vector and then applies it to the 'pushed'.
    /// </summary>
    /// <param name="pusher">The creature doing the pushing</param>
    /// <param name="pushed">The creature being pushed</param>
    /// <param name="magnitude">Magnitude of the push</param>
    public static void PushCreature(ICreature pusher, ICreature pushed, float magnitude)
    {
        Vector3 pushVector = PushVector(pusher.GetCreaturePosition(), pushed.GetCreaturePosition(), magnitude);

        pushed.Push(pushVector);
    }
}