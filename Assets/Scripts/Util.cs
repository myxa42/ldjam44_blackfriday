
using System.Collections.Generic;

public static class Util
{
    public static void Animate(ref long value, long target, long speed)
    {
        long delta = target - value;
        if (delta == 0)
            return;

        long threshold = speed * 60;
        if (delta < 0) {
            if (delta < -speed) {
                if (delta < -threshold)
                    delta /= 8;
                else
                    delta = -speed;
            }
        } else {
            if (delta > speed) {
                if (delta > threshold)
                    delta /= 8;
                else
                    delta = speed;
            }
        }

        value += delta;
    }

    public static void SetArrayItemToNull<T>(IList<T> array, T value)
        where T : class
    {
        int n = array.Count;
        for (int i = 0; i < n; i++) {
            if (array[i] == value) {
                array[i] = null;
                return;
            }
        }
    }
}
