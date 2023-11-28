using System;
using UnityEngine;

public static class Vector3Ext
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.x, z ?? vector.z);
    }
    public static Vector3 With(this Vector3 vector,
        Func<float, float> x = null,
        Func<float, float> y = null,
        Func<float, float> z = null)
    {
        return new Vector3(x?.Invoke(vector.x) ?? vector.x, y?.Invoke(vector.y) ?? vector.y, z?.Invoke(vector.z) ?? vector.z);
    }
}