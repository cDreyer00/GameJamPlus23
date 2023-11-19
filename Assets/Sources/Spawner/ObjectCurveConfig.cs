using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectCurveConfig : ScriptableObject
{
    [SerializeField] SerializableKVP<MonoBehaviour, AnimationCurve> objectCurvePair;
    [SerializeField] float duration;
    [SerializeField] int amount;

    public MonoBehaviour Obj => objectCurvePair.key;
    public AnimationCurve Curve => objectCurvePair.value;
    public float Duration => duration;

    public int GetAmount(float elapsedTime)
    {
        return (int)Curve.Evaluate(elapsedTime);
    }

    void OnValidate()
    {
        duration = Curve.keys[^1].time;
        amount = (int)Curve.keys[^1].value;
    }
}
