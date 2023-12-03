using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ImpactDamage : MonoBehaviour
{
    event Action<Collider> OnImpact;
    void OnValidate() {}
    void OnTriggerEnter(Collider other)
    {
        OnImpact?.Invoke(other);
    }
    public void SetListener(Action<Collider> action)
    {
        if (OnImpact != null) {
            Debug.LogWarning("ImpactDamage already has a listener, Use AddListener instead.");
            return;
        }
        OnImpact = action;
    }
    public void AddListener(Action<Collider> action)
    {
        OnImpact += action;
    }
    public void RemoveListener(Action<Collider> action)
    {
        OnImpact -= action;
    }
}