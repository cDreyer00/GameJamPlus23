using UnityEngine;

public interface IEventEmitter<in TEvent> where TEvent : System.Delegate
{
    void AddListener(TEvent action);
    void RemoveListener(TEvent action);
}