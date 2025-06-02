using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Given a list of hurtboxes, if one of them is hit first, ignore hits on this hurtbox.
/// Useful for hurtboxes on a creature with a large and complex shape.
/// </summary>
public partial class ExclusionHurtbox : Hurtbox
{
    [Export] public Hurtbox[] exclusion;
    [Export] public ParryHitbox parryHitbox;

    public override void HitBy(Hitbox hitbox)
    {
        if (exclusion.Length != 0)
        {
            foreach (Hurtbox hurt in exclusion)
            {
                if (hurt.hit)
                    return;
            }
        }

        if (parryHitbox != null && parryHitbox.OverlapsHitbox())
        {
            return;
        }

        base.HitBy(hitbox);
    }
}