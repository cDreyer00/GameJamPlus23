using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class WaveConfig : ScriptableObject
{
    [SerializeField] ObjectCurveConfig[] objectsList;
    [SerializeField] float duration;

    public ObjectCurveConfig[] ObjectsList => objectsList;
    public float Duration => duration;

    void OnValidate()
    {
        if (objectsList.Length == 0) return;
        
        duration = objectsList.OrderBy(obj => obj.Duration).ToArray()[^1].Duration;
    }

    public MonoBehaviour[] GetObjects() => objectsList.Select(o => o.Obj).ToArray();
}