using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattleConfig : ScriptableObject
{
    [SerializeField] WaveConfig[] waves;
    [SerializeField] float duration;

    public WaveConfig[] Waves => waves;
    public float Duration => duration;

    void OnValidate()
    {
        float totalTimer = 0;
        foreach (WaveConfig w in waves)
            totalTimer += w.Duration;

        duration = totalTimer;
    }

    public MonoBehaviour[][] GetObjects() => waves.Select(w => w.GetObjects()).ToArray();
}