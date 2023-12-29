using System;
public readonly struct Scope
{
    readonly Action _onAcquire;
    readonly Action _onDispose;

    public Scope(Action onAcquire, Action onDispose)
    {
        _onAcquire = onAcquire;
        _onDispose = onDispose;
    }
    public Disposable Begin()
    {
        _onAcquire.Invoke();
        return new Disposable(_onDispose);
    }
}
public readonly struct Scope<T>
{
    readonly Action<T> _onAcquire;
    readonly Action<T> _onDispose;

    public Scope(Action<T> onAcquire, Action<T> onDispose)
    {
        _onAcquire = onAcquire;
        _onDispose = onDispose;
    }
    public Disposable<T> Begin(T arg)
    {
        _onAcquire.Invoke(arg);
        return new Disposable<T>(_onDispose, arg);
    }
}