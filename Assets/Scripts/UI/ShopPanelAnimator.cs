
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopPanelAnimator : MonoBehaviour
{
    public Image DarkOverlay;
    public float Time = 1.0f;

    private UI mUI;
    private RectTransform mTransform;
    private float mOverlayAlpha;
    private float mCurrentTime;
    private float mStartScale;
    private bool mOpen;
    private bool mAnimating;

    public bool IsAnimating => mAnimating;

    void Awake()
    {
        mUI = FindObjectOfType<UI>();
        mTransform = GetComponent<RectTransform>();
        mTransform.localScale = Vector3.zero;
    }

    void Start()
    {
        mOverlayAlpha = 0.8f;//DarkOverlay.color.a; // FIXME
    }

    void Update()
    {
        if (!mAnimating)
            return;

        bool done = false;

        if (mOpen) {
            mCurrentTime += UnityEngine.Time.unscaledDeltaTime;
            if (mCurrentTime >= Time) {
                mCurrentTime = Time;
                done = true;
            }
        } else {
            mCurrentTime -= UnityEngine.Time.unscaledDeltaTime;
            if (mCurrentTime <= 0.0f) {
                mCurrentTime = 0.0f;
                done = true;
            }
        }

        SetTime(mCurrentTime);

        if (done) {
            mAnimating = false;
            mUI.UpdateUI();
        }
    }

    void SetTime(float time)
    {
        float scale = Easing.EaseInOutCubic(time / Time);
        mTransform.localScale = new Vector3(1.0f, scale, 1.0f);

        var color = DarkOverlay.color;
        color.a = mOverlayAlpha * Mathf.Clamp(scale, 0.0f, 1.0f);
        DarkOverlay.color = color;
    }

    public void SetOpen(bool open)
    {
        if (mOpen != open) {
            mOpen = open;
            if (!mAnimating) {
                SetTime(mCurrentTime);
                mAnimating = true;
            }
        }
    }
}
