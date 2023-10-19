using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

#region Pools
[Serializable]
public abstract class GenericPool<T> where T : MonoBehaviour
{
    [Tooltip("Amount instances to be created")]
    [SerializeField] int _amount;
    public int amount => _amount;

    [Tooltip("Reference object")]
    [SerializeField] T _original;
    public T original => _original;

    [Tooltip("(Optional) parent to create the objects")]
    [SerializeField] protected Transform parent = null;

    public GenericPool(T original, int amount, Transform parent = null, bool active = false)
    {
        _original = original;
        _amount = amount;
        this.parent = parent;
    }

    bool _initialized = false;
    public bool initialized => _initialized;

    public void Init()
    {
        if (_initialized) return;
        _initialized = true;

        CreateObjects(original, amount, parent);
    }

    public void SetOriginal(T newOriginal)
    {
        if (_original = newOriginal)
            return;

        if (_original != null)
            MonoBehaviour.Destroy(_original.gameObject);

        _original = newOriginal;
    }

    protected abstract void CreateObjects(T original, int amount, Transform parent = null, bool active = false);
    public abstract T Get(Vector3 position, Quaternion rotation);
    public abstract T Get(Transform parent);
    public abstract void Release(T obj);
    public abstract void Dispose();

    public void SetInit(bool init) => _initialized = init;
}

#region Queue Pool
[Serializable]
public class QueuePool<T> : GenericPool<T> where T : MonoBehaviour
{
    public Queue<T> q = new();

    public QueuePool(T original, int amount, Transform parent = null, bool active = false) : base(original, amount, parent, active)
    {

    }

    protected override void CreateObjects(T original, int amount, Transform parent = null, bool active = false)
    {
        if (q == null) q = new();

        for (int i = 0; i < amount; i++)
        {
            T obj = UnityEngine.Object.Instantiate(original, parent);
            obj.gameObject.SetActive(active);

            if (obj is IPoolable<T> poolable)
            {
                poolable.OnCreated();
            }

            q.Enqueue(obj);
        }
    }

    public override T Get(Vector3 position, Quaternion rotation)
    {
        if (q.Count == 0)
        {
            CreateObjects(original, 1, parent);
        }

        T t = q.Dequeue();

        t.transform.SetParent(null);

        t.transform.position = position;
        t.transform.rotation = rotation;

        t.gameObject.SetActive(true);

        if (t is IPoolable<T> poolable)
        {
            poolable.OnGet(this);
        }

        return t;
    }

    public override T Get(Transform parent)
    {
        if (q.Count == 0)
        {
            CreateObjects(original, 1, parent);
        }

        T t = q.Dequeue();

        t.transform.SetParent(parent);

        t.transform.localPosition = Vector3.zero;
        t.transform.localEulerAngles = Vector3.zero;

        t.gameObject.SetActive(true);

        if (t is IPoolable<T> poolable)
        {
            poolable.OnGet(this);
        }

        return t;
    }

    T obj;
    public override void Release(T obj)
    {
        this.obj = obj;
        this.obj.gameObject.SetActive(false);
        this.obj.transform.SetParent(parent);

        if (this.obj is IPoolable<T> poolable)
        {
            poolable.OnRelease();
        }

        q.Enqueue(this.obj);
    }

    public override void Dispose()
    {
        if (q == null || q.Count == 0) return;

        while (q.Count > 0)
        {
            T t = q.Dequeue();
            UnityEngine.Object.Destroy(t.gameObject);
        }
    }
}

#endregion

#region Stack Pool
[Serializable]
public class StackPool<T> : GenericPool<T> where T : MonoBehaviour
{
    public Stack<T> s = new();

    public StackPool(T original, int amount, Transform parent = null, bool active = false) : base(original, amount, parent, active)
    {

    }

    protected override void CreateObjects(T original, int amount, Transform parent = null, bool active = false)
    {
        if (s == null) s = new();

        for (int i = 0; i < amount; i++)
        {
            T obj = UnityEngine.Object.Instantiate(original, parent);
            obj.gameObject.SetActive(active);

            if (obj is IPoolable<T> poolable)
            {
                poolable.OnCreated();
            }

            s.Push(obj);
        }
    }

    public override T Get(Vector3 position, Quaternion rotation)
    {
        if (s.Count == 0)
        {
            CreateObjects(original, 1, parent);
        }

        T t = s.Pop();

        t.transform.SetParent(null);

        t.transform.position = position;
        t.transform.rotation = rotation;

        t.gameObject.SetActive(true);

        if (t is IPoolable<T> poolable)
        {
            poolable.OnGet(this);
        }

        return t;
    }

    public override T Get(Transform parent)
    {
        if (s.Count == 0)
        {
            CreateObjects(original, 1, parent);
        }

        T t = s.Pop();

        t.transform.SetParent(parent);

        t.transform.localPosition = Vector3.zero;
        t.transform.localEulerAngles = Vector3.zero;

        t.gameObject.SetActive(true);

        if (t is IPoolable<T> poolable)
        {
            poolable.OnGet(this);
        }

        return t;
    }

    public override void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);

        if (obj is IPoolable<T> poolable)
        {
            poolable.OnRelease();
        }

        s.Push(obj);
    }

    public override void Dispose()
    {
        if (s == null || s.Count == 0) return;

        while (s.Count > 0)
        {
            T t = s.Pop();
            UnityEngine.Object.Destroy(t.gameObject);
        }
    }
}
#endregion

public interface IPoolable<T> where T : MonoBehaviour
{
    public GenericPool<T> Pool { get; set; }
    public void OnGet(GenericPool<T> pool);
    public void OnRelease();
    public void OnCreated();
}
#endregion