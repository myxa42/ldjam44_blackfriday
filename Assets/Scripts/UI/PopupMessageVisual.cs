
using UnityEngine;
using TMPro;

public sealed class PopupMessageVisual : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private RectTransform mRectTransform;
    private PopupMessage mOwner;
    private Vector2 mInitialPosition;
    private Color mColor;
    private float mCurrentTime;

    void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
        mOwner = GetComponentInParent<PopupMessage>();
        mInitialPosition = mRectTransform.anchoredPosition;
    }

    void Update()
    {
        bool done = false;

        mCurrentTime += Time.deltaTime;
        if (mCurrentTime >= mOwner.TotalTime) {
            mCurrentTime = mOwner.TotalTime;
            done = true;
        }

        float t = mCurrentTime / mOwner.TotalTime;
        switch (mOwner.AnimationType) {
            case PopupMessage.Animation.FlyUp:
                mRectTransform.anchoredPosition = mInitialPosition + mOwner.DirectionAndDistance * t;
                break;

            case PopupMessage.Animation.ZoomIn: {
                float scale = Easing.EaseOutBack(t);
                mRectTransform.localScale = new Vector3(scale, scale, scale);
                break;
            }
        }

        if (mCurrentTime > mOwner.TimeBeforeFade) {
            float coeff = 1.0f - (mCurrentTime - mOwner.TimeBeforeFade) / (mOwner.TotalTime - mOwner.TimeBeforeFade);
            Text.color = new Color(mColor.r, mColor.g, mColor.b, mColor.a * coeff);
        }

        if (done)
            mOwner.AddInactiveVisual(this);
    }

    public void BeginAnimating(string message, Color color)
    {
        gameObject.SetActive(true);
        mRectTransform.anchoredPosition = mInitialPosition;

        switch (mOwner.AnimationType) {
            case PopupMessage.Animation.FlyUp:
                mRectTransform.localScale = Vector3.one;
                break;

            case PopupMessage.Animation.ZoomIn:
                mRectTransform.localScale = Vector3.zero;
                break;
        }

        mColor = color;
        mCurrentTime = 0.0f;
        Text.text = message;
        Text.color = color;
    }
}
