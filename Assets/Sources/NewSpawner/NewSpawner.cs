using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpawner : MonoBehaviour
{
    [SerializeField] WaveConfig[] waves;

    [SerializeField] float elapsedTime;

    [ContextMenu("Test")]
    public void Test()
    {
        int total = waves[0].objects[0].GetAmount(elapsedTime, waves[0].duration);
        GameLogger.Log(total);
    }
}

[Serializable]
public class WaveConfig
{
    public WaveObject[] objects;
    public float duration;
}

[Serializable]
public struct WaveObject
{
    public MonoBehaviour obj;
    public AnimationCurve amountCurve;

    public int GetAmount(float elapsedTime, float totalTimer)
    {
        float value = elapsedTime * 100 / totalTimer / 100;
        int curveValue = (int)amountCurve.Evaluate(value);
        return curveValue;
    }
}