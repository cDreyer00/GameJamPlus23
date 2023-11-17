using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpawnFrequency : ScriptableObject
{
    [SerializeField] SerializableKVP<MonoBehaviour, AnimationCurve> objectCurvePair;
    [SerializeField] float duration;

    public MonoBehaviour Obj => objectCurvePair.key;
    public AnimationCurve Curve => objectCurvePair.value;
    public float Duration => duration;

    public (MonoBehaviour, int) GetAmount(float elapsedTime)
    {
        return (Obj, (int)Curve.Evaluate(elapsedTime));
    }

    void OnValidate()
    {
        duration = Curve.keys[^1].time;
    }
}

[Serializable]
public struct SerializableKVP<TKey, TValue>
{
    public TKey key;
    public TValue value;

    public SerializableKVP(TKey key, TValue value)
    {
        this.key = key;
        this.value = value;
    }
}