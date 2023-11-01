using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;
using Sources.Types;
using UnityEngine.Serialization;

namespace Sources.Enemy
{
    public abstract class BaseSpawner<T> : Singleton<BaseSpawner<T>>, ISpawner<T>
        where T : MonoBehaviour
    {
        protected QueuePool<T> instances;
        public IEnumerable<T> Instances => instances.q;
        public T                       instancePrefab;
        public int                     startSpawnTimer;
        public ClampedPrimitive<float> spawnRate;
        public ClampedPrimitive<int>   maxInstances;

        [SerializeField] int activeInstances;

        virtual protected void Start()
        {
            instances = new QueuePool<T>(instancePrefab, maxInstances.max, transform);
            instances.Init();
            StartCoroutine(SpawnCoroutine());
        }
        public abstract Vector3 GetRandomPosition();
        public IEnumerator SpawnCoroutine()
        {
            yield return Helpers.GetWait(startSpawnTimer);

            var wait = Helpers.GetWait(spawnRate);
            while (true) {
                if (activeInstances >= maxInstances) {
                    yield return null;
                    continue;
                }
                yield return wait;
                Spawned();
            }
        }
        protected void Spawned()
        {
            activeInstances++;
            var position = GetRandomPosition();
            T   instance = instances.Get(position, Quaternion.identity);
            OnSpawned(instance);
        }
        protected void DeSpawned(T instance)
        {
            activeInstances--;
            instances.Release(instance);
            OnDesSpawned(instance);
        }
        virtual protected void OnSpawned(T instance) {}
        virtual protected void OnDesSpawned(T instance) {}
    }
}