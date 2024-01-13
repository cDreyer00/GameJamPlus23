using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Collider))]
public class CollisionEmitter : MonoBehaviour, IEventEmitter<Action<Object, Collider>>
{
    event Action<Object, Collider> Event;
    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        Event?.Invoke(this, other);
    }
    public void AddListener(Action<Object, Collider> action)
    {
        Event += action;
    }
    public void RemoveListener(Action<Object, Collider> action)
    {
        Event -= action;
    }
}