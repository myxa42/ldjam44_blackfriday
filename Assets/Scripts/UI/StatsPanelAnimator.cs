
using UnityEngine;

public sealed class StatsPanelAnimator : MonoBehaviour
{
    public RectTransform TopmostContainer;
    public float Time = 1.0f;

    private RectTransform mTransform;
    private RectTransform mTargetTransform;
    private Transform mTargetTransformParent;
    private float mCurrentTime;
    private Vector2 mStartPosition;
    private bool mAnimating;

    void Update()
    {
        if (!mAnimating)
            return;

        bool done = false;

        mCurrentTime += UnityEngine.Time.unscaledDeltaTime;
        if (mCurrentTime >= Time) {
            mCurrentTime = Time;
            done = true;
        }

        float t = 1.0f - mCurrentTime / Time;
        mTransform.anchoredPosition = mStartPosition * Easing.EaseInOutCubic(t);

        if (done) {
            mAnimating = false;
            mTargetTransform.SetParent(mTargetTransformParent);
            mTargetTransform = null;
        }
    }

    public void SetTargetParent(RectTransform target)
    {
        if (mTransform == null)
            mTransform = GetComponent<RectTransform>();

        if (mTargetTransform != null)
            mTargetTransform.SetParent(mTargetTransformParent);

        mTargetTransform = target;
        mTargetTransformParent = target.parent;
        target.SetParent(TopmostContainer);
        mTransform.SetParent(target);

        mStartPosition = mTransform.anchoredPosition;
        mCurrentTime = 0.0f;

        mAnimating = true;
    }
}
