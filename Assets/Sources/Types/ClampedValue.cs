using System;
using UnityEngine;

[Serializable]
public struct ClampedPrimitive<TValue> where TValue : unmanaged, IComparable<TValue>, IEquatable<TValue>
{
    [SerializeField] TValue value;
    public           TValue min;
    public           TValue max;
    public ClampedPrimitive(TValue value, TValue min, TValue max)
    {
        this.value = Clamp(value, min, max);
        this.min = min;
        this.max = max;
    }
    public void Clamp() => value = Clamp(value, min, max);
    public TValue Value
    {
        get => value = Clamp(value, min, max);
        set => this.value = Clamp(value, min, max);
    }
    public static TValue Clamp(TValue value, TValue min, TValue max)
    {
        if (value.CompareTo(min) <= 0) {
            return min;
        }
        else if (value.CompareTo(max) >= 0) {
            return max;
        }
        return value;
    }

    public static implicit operator TValue(ClampedPrimitive<TValue> primitive) => primitive.Value;
}