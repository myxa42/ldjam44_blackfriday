
using UnityEngine;

public static class Easing
{
    public static float EaseInOutCubic(float x)
    {
        if (x < 0.5f)
            return 4.0f * x * x * x;
        else {
            float f = 2.0f * x - 2.0f;
            return 1.0f + 0.5f * f * f * f;
        }
    }

    public static float EaseOutBack(float x)
    {
        float f = 1.0f - x;
        return 1.0f - (f * f * f - f * Mathf.Sin(f * Mathf.PI));
    }
}
