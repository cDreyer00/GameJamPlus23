using System;

public readonly struct Disposable : IDisposable
{
    readonly Action _onDispose;

    public Disposable(Action onDispose)
    {
        _onDispose = onDispose;
    }

    public void Dispose()
    {
        _onDispose?.Invoke();
    }
}
public readonly struct Disposable<T> : IDisposable
{
    readonly T         _context;
    readonly Action<T> _onDispose;

    public Disposable(Action<T> onDispose, T context)
    {
        _onDispose = onDispose;
        _context = context;
    }

    public void Dispose()
    {
        _onDispose?.Invoke(_context);
    }
}