using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Wave : ScriptableObject
{
    [SerializeField] SpawnFrequency[] objectsList;
    [SerializeField] float duration;

    public SpawnFrequency[] ObjectsList => objectsList;
    public float Duration => duration;

    public int GetAmount(float elapsedTime)
    {
        (MonoBehaviour nill, int amount) = objectsList[0].GetAmount(elapsedTime);
        return amount;
    }

    void OnValidate()
    {
        float totalTimer = 0;
        foreach (var objs in objectsList)
            totalTimer += objs.Duration;

        duration = totalTimer;
    }
}