
using UnityEngine;
using System.Collections.Generic;

public sealed class PopupMessage : MonoBehaviour
{
    public enum Animation
    {
        FlyUp,
        ZoomIn,
    }

    public Animation AnimationType;
    public PopupMessageVisual Prefab;
    public Vector2 DirectionAndDistance = new Vector2(0.0f, 1.0f);
    public float TotalTime;
    public float TimeBeforeFade;
    public int Preallocate = 5;

    private readonly Stack<PopupMessageVisual> mInactiveVisuals = new Stack<PopupMessageVisual>();

    void Awake()
    {
        var t = transform;
        for (int i = 0; i < Preallocate; i++)
            AddInactiveVisual(Instantiate(Prefab, t));
    }

    public void ShowMessage(string message, Color color)
    {
        PopupMessageVisual visual = null;
        while (mInactiveVisuals.Count > 0) {
            visual = mInactiveVisuals.Pop();
            if (visual != null)
                break;
        }

        if (visual == null)
            visual = Instantiate(Prefab, transform);

        visual.BeginAnimating(message, color);
    }

    public void AddInactiveVisual(PopupMessageVisual visual)
    {
        visual.gameObject.SetActive(false);
        mInactiveVisuals.Push(visual);
    }
}
