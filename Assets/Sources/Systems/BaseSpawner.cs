using System.Collections;
using System.Collections.Generic;
using Sources.cdreyer;
using Sources.cdreyer.GenericPool;
using Sources.Types;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources.Systems
{
    public abstract class BaseSpawner<T> : Singleton<BaseSpawner<T>>
        where T : MonoBehaviour
    {
        public T                       prefab;
        public ClampedPrimitive<float> spawnRate;
        public ClampedPrimitive<int>   maxInstances;
        public bool                    shouldSpawn;

        protected QueuePool<T> instancePool;
        public abstract Vector3 GetRandomPosition();
        protected override void Awake()
        {
            base.Awake();
            instancePool = new QueuePool<T>(prefab, maxInstances.max, transform);
            spawnRate.Clamp();
            maxInstances.Clamp();
            BeginSpawning();
        }
        public void BeginSpawning()
        {
            shouldSpawn = true;
            instancePool.Init();
            StartCoroutine(SpawnCoroutine());
        }
        public void StopSpawning()
        {
            shouldSpawn = false;
            StopAllCoroutines();
        }
        IEnumerator SpawnCoroutine()
        {
            if (!shouldSpawn) yield break;

            var wait = Helpers.GetWait(spawnRate);
            while (shouldSpawn) {
                yield return wait;
                Spawn();
            }
        }
        virtual protected void Spawn()
        {
            var position = GetRandomPosition();
            T   instance = instancePool.Get(position, Quaternion.identity);
            OnSpawnedInstance(instance);
        }
        protected void DeSpawn(T instance)
        {
            instancePool.Release(instance);
            OnDesSpawnedInstance(instance);
        }
        virtual protected void OnSpawnedInstance(T instance) {}
        virtual protected void OnDesSpawnedInstance(T instance) {}
    }
}