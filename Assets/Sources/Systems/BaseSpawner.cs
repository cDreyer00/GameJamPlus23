using System;
using System.Collections;
using System.Collections.Generic;
using CDreyer;
using Sources.Enemy;
using Sources.Types;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Systems
{
    public abstract class BaseSpawner<T> : Singleton<BaseSpawner<T>>
        where T : MonoBehaviour
    {
        protected QueuePool<T> instances;
        public IEnumerable<T> Instances => instances.q;
        public T                       instancePrefab;
        public int                     startSpawnTimer;
        public ClampedPrimitive<float> spawnRate;
        public ClampedPrimitive<int>   maxInstances;
        public bool                    cancellationRequested;

        [SerializeField] protected int activeInstances;

        public abstract Vector3 GetRandomPosition();

        protected override void Awake()
        {
            base.Awake();
            instances = new QueuePool<T>(instancePrefab, maxInstances.max, transform);
            spawnRate.Clamp();
            maxInstances.Clamp();
            Start();
        }

        public void Start()
        {
            cancellationRequested = false;
            instances.Init();
            StartCoroutine(SpawnCoroutine());
        }
        public void Stop()
        {
            cancellationRequested = true;
            StopAllCoroutines();
        }
        virtual protected IEnumerator SpawnCoroutine()
        {
            if (!cancellationRequested) yield break;
            yield return Helpers.GetWait(startSpawnTimer);

            var wait = Helpers.GetWait(spawnRate);
            while (!cancellationRequested) {
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