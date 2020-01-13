using UnityEngine;

public static class ExtensionMethods
{

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Fract(this float value)
    {
        return value - Mathf.Floor(value);
    }

    public static Vector3 Limit(this Vector3 value, float max)
    {
        if (value.magnitude > max)
        {
            value.Normalize();
            value *= max;
        }

        return value;
    }
}