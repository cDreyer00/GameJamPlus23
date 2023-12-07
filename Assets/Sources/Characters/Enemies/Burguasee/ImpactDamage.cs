using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ImpactDamage : MonoBehaviour
{
    event Action<Collider> OnImpact;
    void OnTriggerEnter(Collider other)
    {
        OnImpact?.Invoke(other);
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