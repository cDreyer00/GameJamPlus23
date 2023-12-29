using System;
using System.Collections.Generic;
using System.Reflection;

public static class Cached
{
    readonly static Dictionary<Type, Array> EnumCache = new();
    public static T[] EnumValues<T>() where T : Enum
    {
        if (!EnumCache.TryGetValue(typeof(T), out var values)) {
            values = Enum.GetValues(typeof(T));
            EnumCache.Add(typeof(T), values);
        }
        return (T[])values;
    }
}