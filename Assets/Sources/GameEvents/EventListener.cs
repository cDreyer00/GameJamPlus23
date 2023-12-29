using System;
using Action = System.Action;
using Object = UnityEngine.Object;
[Serializable]
public struct EventListener
{
    object _event;
    public EventListener(Action action) => _event = action;
    public EventListener(UnityEventResponse unityEvent) => _event = unityEvent;
    public void SetEvent(Action action) => _event = action;
    public void SetEvent<TSender, TArg>(Action<TSender, TArg> action) where TSender : Object => _event = action;
    public void SetEvent(UnityEventResponse unityEvent) => _event = unityEvent;
    public void OnInvoke<TSender, TArgs>(TSender sender, TArgs args)
        where TSender : Object
    {
        switch (_event) {
        case UnityEventResponse eventResponse:
            eventResponse.Invoke(sender, args);
            break;
        case Action<TSender, TArgs> eventResponse:
            eventResponse.Invoke(sender, args);
            break;
        case Action ac2:
            ac2.Invoke();
            break;
        }
    }
}