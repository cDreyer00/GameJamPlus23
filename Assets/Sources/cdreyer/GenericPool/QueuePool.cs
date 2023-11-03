using System;
using System.Collections.Generic;
using UnityEngine;

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
                InstanceCreated(obj);
            }
        }

        public override T Get(Vector3 position, Quaternion rotation)
        {
            if (q.Count == 0)
            {
                CreateObjects(Original, 1, parent);
            }

            T t = q.Dequeue();

            var transform = t.transform;
            transform.SetParent(null);

            transform.position = position;
            transform.rotation = rotation;

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
            if (q.Count == 0)
            {
                CreateObjects(Original, 1, parent);
            }

            T t = q.Dequeue();

            var transform = t.transform;
            transform.SetParent(parent);

            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

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
            q.Enqueue(obj);
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