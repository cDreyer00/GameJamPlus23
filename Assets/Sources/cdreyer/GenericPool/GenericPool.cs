using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public abstract class GenericPool<T> where T : MonoBehaviour
{
    [SerializeField] int _amount;
    [SerializeField] T _original;
    [SerializeField] protected Transform parent = null;

    public event Action<T> onInstanceCreated;
    public event Action<T> onInstanceReleased;
    public event Action<T> onInstanceTaken;

    bool _initialized = false;

    public GenericPool(T original, int amount, Transform parent = null)
    {
        _original = original;
        _amount = amount;
        this.parent = parent;
    }

    public int Amount => _amount;
    public T Original => _original;
    public Transform Parent => parent;
    public bool Initialized { get => _initialized; set => _initialized = value; }

    public void Init()
    {
        if (_initialized) return;
        _initialized = true;

        CreateObjects(Original, Amount, parent);
    }

    public void SetOriginal(T newOriginal)
    {
        if (_original = newOriginal)
            return;

        if (_original != null)
            Object.Destroy(_original.gameObject);

        _original = newOriginal;
    }

    protected void InstanceCreated(T i) => onInstanceCreated?.Invoke(i);
    protected void InstanceTaken(T i) => onInstanceTaken?.Invoke(i);
    protected void InstanceReleased(T i) => onInstanceReleased?.Invoke(i);

    protected abstract void CreateObjects(T original, int amount, Transform parent = null, bool active = false);
    public abstract T Get(Vector3 position, Quaternion rotation);
    public abstract T Get(Transform parent);
    public abstract void Release(T obj);
    public abstract void Dispose();
}