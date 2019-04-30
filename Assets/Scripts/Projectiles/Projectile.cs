
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool DeleteWhenDone;

    private ProjectileShooter mOwner;
    private Vector3 mOriginalPosition;
    private Vector3 mTargetPosition;
    private Quaternion mOriginalRotation;
    private Quaternion mTargetRotation;
    private float mTotalTime = 1.0f;
    private float mCurrentTime = 1.0f;

    void Awake()
    {
        mOwner = GetComponentInParent<ProjectileShooter>();
    }

    public void BeginAnimating(Vector3 targetPosition, float time)
    {
        var t = transform;
        gameObject.SetActive(true);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.SetParent(null);
        mOriginalPosition = t.position;
        mOriginalRotation = t.rotation;
        mTargetPosition = targetPosition;
        mTargetRotation = Quaternion.LookRotation(mTargetPosition - mOriginalPosition);
        mTotalTime = time;
        mCurrentTime = 0.0f;
        ResetAnimation();
    }

    void Update()
    {
        bool done = false;

        mCurrentTime += Time.deltaTime;
        if (mCurrentTime >= mTotalTime) {
            mCurrentTime = mTotalTime;
            done = true;
        }

        float t = mCurrentTime / mTotalTime;
        transform.position = AnimatePosition(mOriginalPosition, mTargetPosition, t);
        transform.rotation = AnimateRotation(mOriginalRotation, mTargetRotation, t);
        Animate(t);

        if (done) {
            if (DeleteWhenDone)
                Destroy(gameObject);
            else {
                transform.SetParent(mOwner.transform);
                mOwner.AddInactiveProjectile(this);
            }
        }
    }

    protected virtual void ResetAnimation()
    {
    }

    protected virtual Vector3 AnimatePosition(Vector3 from, Vector3 to, float t)
    {
        return Vector3.Lerp(from, to, t);
    }

    protected virtual Quaternion AnimateRotation(Quaternion from, Quaternion to, float t)
    {
        return Quaternion.Slerp(from, to, t);
    }

    protected virtual void Animate(float time)
    {
    }
}
