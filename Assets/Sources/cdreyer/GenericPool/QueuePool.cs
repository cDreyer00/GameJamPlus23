using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDreyer
{
    [Serializable]
    public class QueuePool<T> : GenericPool<T> where T : MonoBehaviour
    {
        public Queue<T> q = new();

        public QueuePool(T original, int amount, Transform parent = null) : base(original, amount, parent)
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
                CreateObjects(Original, 1, parent);
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
                CreateObjects(Original, 1, parent);
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
}