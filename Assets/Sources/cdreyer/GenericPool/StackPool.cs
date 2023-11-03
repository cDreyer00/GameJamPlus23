using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StackPool<T> : GenericPool<T> where T : MonoBehaviour
{
    public Stack<T> s = new();

    public StackPool(T original, int amount, Transform parent = null) : base(original, amount, parent)
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

            InstanceCreated(obj);
            s.Push(obj);
        }
    }

    public override T Get(Vector3 position, Quaternion rotation)
    {
        if (s.Count == 0)
        {
            CreateObjects(Original, 1, parent);
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

        InstanceTaken(t);
        return t;
    }

    public override T Get(Transform parent)
    {
        if (s.Count == 0)
        {
            CreateObjects(Original, 1, parent);
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

        InstanceTaken(t);
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

        InstanceReleased(obj);
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