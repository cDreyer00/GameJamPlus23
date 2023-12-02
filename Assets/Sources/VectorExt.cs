using System;
using UnityEngine;

public static class Vector3Ext
{
    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null) =>
        new(x ?? vector.x, y ?? vector.x, z ?? vector.z);
    public static float SqrDistance(Vector3 vector, Vector3 other) => (vector - other).sqrMagnitude;
    public static void SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null) =>
        transform.position = transform.position.With(x, y, z);
    public static void SetLocalPosition(this Transform transform, float? x = null, float? y = null, float? z = null) =>
        transform.localPosition = transform.localPosition.With(x, y, z);
    public static Vector3 Direction(Vector3 source, Vector3 destination) => (destination - source).normalized;
}