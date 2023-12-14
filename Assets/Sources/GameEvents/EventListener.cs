using System;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[Serializable]
public struct EventListener : IEquatable<EventListener>
{
    public ScriptableObjectEvent eventSource;
    public object                eventResponse;
    EventListener(ScriptableObjectEvent eventSource, object eventResponse)
    {
        this.eventSource = eventSource;
        this.eventResponse = eventResponse;
    }
    public static EventListener CreateInstance(Action action)
    {
        return new(ScriptableObject.CreateInstance<ScriptableObjectEvent>(), action);
    }
    public static EventListener CreateInstance<TSender, TArg>(Action<TSender, TArg> action)
        where TSender : Object
        where TArg : class
    {
        return new(ScriptableObject.CreateInstance<ScriptableObjectEvent>(), action);
    }
    public static EventListener CreateBinding<TSender, TArg>(ScriptableObjectEvent eventSource, Action<TSender, TArg> delegateResponse)
        where TSender : Object
        where TArg : class
    {
        return new(eventSource, delegateResponse);
    }
    public static EventListener CreateBinding(ScriptableObjectEvent eventSource, Action delegateResponse)
    {
        return new(eventSource, delegateResponse);
    }
    public static EventListener CreateBinding<TSender, TArg>(ScriptableObjectEvent eventSource,
        UnityEvent<TSender, TArg> eventResponse)
        where TSender : Object
        where TArg : class
    {
        return new(eventSource, eventResponse);
    }
    public static EventListener CreateBinding<TSender>(ScriptableObjectEvent eventSource, UnityEvent<TSender> eventResponse)
        where TSender : Object
    {
        return new(eventSource, eventResponse);
    }
    public void OnInvoke<TSender, TArgs>(TSender sender, TArgs args)
        where TSender : Object
        where TArgs : class
    {
        switch (eventResponse) {
        case UnityEvent<TSender, TArgs> ue0:
            ue0.Invoke(sender, args);
            break;
        case UnityEvent ue2:
            ue2.Invoke();
            break;
        case Action<TSender, TArgs> ac0:
            ac0.Invoke(sender, args);
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