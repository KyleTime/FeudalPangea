using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class FireballObject : Node3D
{
    public Vector3 velocity;
    [Export] Area3D groundbox;
    [Export] Hitbox hitbox;

    public Vector3 target;
    public Vector3 direction;
    public float speed;
    bool useTargetting = false;

    public override void _Ready()
    {
        groundbox.BodyShapeEntered += CollideWithGround;
        hitbox.HitHurtBox += HitHurtBox;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (useTargetting)
        {
            GD.Print(direction);
            if (KMath.Dist3D(target, GlobalPosition) < 1)
            {
                useTargetting = false;
            }

            velocity = (target - GlobalPosition).Normalized() * speed;
        }
        else
        {
            velocity = direction.Normalized() * speed;
            GD.Print("vel: " + velocity);
        }

        Position += velocity * (float)delta;
    }

    public void HitHurtBox(Hurtbox hurtbox)
    {
        if (hurtbox.self is Player)
        {
            hitbox.SetDeferred(Hitbox.PropertyName.hitsPlayer, true);
        }
    }

    public void Activate(Vector3 direction, float speed, Vector3 target)
    {
        Activate(true);

        this.direction = direction;
        this.speed = speed;
        this.target = target;
    }

    public void Activate(bool target = false)
    {
        hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, false);
        Visible = true;
        hitbox.hitsPlayer = false;
        useTargetting = target;
        direction = new Vector3();
    }

    private void CollideWithGround(Rid bodyRid, Node3D body, long bodyShapeIndex, long localShapeIndex)
    {
        Visible = false;
        velocity = new Vector3();
        hitbox.collider.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
    }
}
