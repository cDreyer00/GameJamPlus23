using System;
using System.Collections.Generic;

public static class Cached
{
    readonly static Dictionary<Type, Array> Cache = new();
    public static Array EnumValues<T>() where T : Enum
    {
        if (!Cache.TryGetValue(typeof(T), out var values)) {
            values = Enum.GetValues(typeof(T));
            Cache.Add(typeof(T), values);
        }
        return values;
    }
}