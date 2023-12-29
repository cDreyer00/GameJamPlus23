using System;
using System.IO;
using UnityEditor;

public static class GameEvents
{
    const string GameEventsBasePath = "Assets/Sources/GameEvents";
    public static LazyLoad<ScriptableObjectEvent> onPause = new(static () =>
        AssetDatabase.LoadAssetAtPath<ScriptableObjectEvent>(Path.Combine(GameEventsBasePath, "Pause.asset"))
    );
    public static LazyLoad<ScriptableObjectEvent> onGameOver = new(static () =>
        AssetDatabase.LoadAssetAtPath<ScriptableObjectEvent>(Path.Combine(GameEventsBasePath, "GameOver.asset"))
    );
}

public struct LazyLoad<T>
{
    readonly Func<T> _factory;
    T                _value;
    public LazyLoad(Func<T> factory)
    {
        _factory = factory;
        _value = default;
    }
    public static implicit operator T(LazyLoad<T> lazyLoad) => lazyLoad._value;
    public static implicit operator LazyLoad<T>(T value) => new() { _value = value };

    public T Value
    {
        get
        {
            if (_value == null)
                _value = _factory();
            return _value;
        }
        set => _value = value;
    }
}