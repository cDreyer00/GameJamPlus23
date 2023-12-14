using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct EventListener : IEquatable<EventListener>
{
    public ScriptableObjectEvent eventSource;
    public object                eventResponse;
    public static EventListener CreateInstance() => new(
        ScriptableObject.CreateInstance<ScriptableObjectEvent>(),
        (Action)delegate {}
    );
    public EventListener(ScriptableObjectEvent eventSource, UnityEventResponse eventResponse)
    {
        this.eventSource = eventSource;
        this.eventResponse = eventResponse;
    }
    public EventListener(ScriptableObjectEvent eventSource, Delegate actionEventResponse)
    {
        this.eventSource = eventSource;
        eventResponse = actionEventResponse;
    }
    public void OnInvoke<TSender, TArgs>(TSender sender, TArgs args)
    {
        switch (eventResponse) {
        case UnityEvent<TSender, TArgs> ue0:
            ue0.Invoke(sender, args);
            break;
        case UnityEvent<TSender> ue1:
            ue1.Invoke(sender);
            break;
        case UnityEvent ue2:
            ue2.Invoke();
            break;
        case Action<TSender, TArgs> ac0:
            ac0.Invoke(sender, args);
            break;
        case Action<TSender> ac1:
            ac1.Invoke(sender);
            break;
        case Action ac2:
            ac2.Invoke();
            break;
        }
    }
    public bool Equals(EventListener other) => eventSource == other.eventSource && eventResponse == other.eventResponse;
    public override bool Equals(object obj) => obj is EventListener other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(eventSource, eventResponse);
}