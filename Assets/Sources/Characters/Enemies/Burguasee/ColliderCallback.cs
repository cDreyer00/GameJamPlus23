using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderCallback : MonoBehaviour
{
    event Action<Collider> TriggerEventCallback;
    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        TriggerEventCallback?.Invoke(other);
    }
    public void AddListener(Action<Collider> action)
    {
        TriggerEventCallback += action;
    }
    public void RemoveListener(Action<Collider> action)
    {
        TriggerEventCallback -= action;
    }
}