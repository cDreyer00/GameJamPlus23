using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalInstances<TObject> where TObject : Object
{
    public Dictionary<string, TObject> instancesDict = new();

    public GlobalInstances(params (string key, TObject instance)[] instances)
    {
        foreach (var kvp in instances)
            AddInstance(kvp.key, kvp.instance);
    }

    public TTarget GetInstance<TTarget>(string key) where TTarget : TObject
    {
        if (!instancesDict.TryGetValue(key, out TObject instance))
            return null;

        if (instance is not TTarget)
        {
            Debug.LogError($"trying to access instance with key {key} as {typeof(TTarget).Name}, instead the type found was {instance.GetType().Name}");
            return null;
        }

        return instance as TTarget;
    }

    public TTarget AddInstance<TTarget>(string key, TTarget instance) where TTarget : TObject
    {
        var i = GetInstance<TTarget>(key);
        if (i != null)
            return i;

        instancesDict[key] = instance;
        return instance;
    }
}
