
using UnityEngine;

public sealed class SpinningProjectile : Projectile
{
    public float RotationAmount = 360.0f * 4;

    protected override void ResetAnimation()
    {
        var t = transform;
        var angles = t.eulerAngles;
        angles.z += 0.0f;
        t.eulerAngles = angles;
    }

    protected override void Animate(float time)
    {
        var t = transform;
        var angles = t.eulerAngles;
        angles.z += RotationAmount * time;
        t.eulerAngles = angles;
    }
}
