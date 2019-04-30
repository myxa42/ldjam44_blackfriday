
using UnityEngine;
using System.Collections.Generic;

public sealed class ProjectileShooter : MonoBehaviour
{
    private readonly Stack<Projectile> mPool = new Stack<Projectile>();

    public Projectile ProjectilePrefab;
    public bool SameY = true;
    public int Preallocate = 2;

    void Awake()
    {
        var t = transform;
        for (int i = 0; i < Preallocate; i++)
            AddInactiveProjectile(Instantiate(ProjectilePrefab, t));
    }

    public Projectile ShootProjectile(Vector3 targetPosition, float time)
    {
        Projectile projectile = null;
        while (mPool.Count > 0) {
            projectile = mPool.Pop();
            if (projectile != null)
                break;
        }

        if (projectile == null)
            projectile = Instantiate(ProjectilePrefab, transform);

        if (SameY)
            targetPosition.y = transform.position.y;

        projectile.BeginAnimating(targetPosition, time);
        return projectile;
    }

    public void AddInactiveProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
        mPool.Push(projectile);
    }
}
