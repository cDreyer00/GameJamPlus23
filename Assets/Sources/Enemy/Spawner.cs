using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CDreyer;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Sources.Enemy
{
    public abstract class Spawner<T> : CDreyer.Singleton<Spawner<T>> where T : MonoBehaviour
    {
        public float difficultySpike;

        public int startSpawnTimer;
        public int minInstanceCount = 1;
        public int maxInstanceCount = 10;
        public int minDamage = 1;
        public int maxDamage = 10;
        public float minSpawnInterval = 0.1f;
        public float maxSpawnInterval = 1f;
        public float spikeFrequency = 60f;
        public float spawnInterval = 1f;
        public float elevation = 1;

        public T instancePrefab;

        protected List<T> instances = new();
        protected int currentMaxInstances = new();
        public List<T> Instances => instances;

        protected override void Awake()
        {
            base.Awake();
            instances = new List<T>();
        }

        protected virtual void Start()
        {
            StartCoroutine(WaitAndSpawn());
        }

        IEnumerator WaitAndSpawn()
        {
            while (true)
            {
                if (GameManager.Instance.GameElapsedTime < startSpawnTimer)
                {
                    yield return null;
                    continue;
                }

                if (instances.Count >= currentMaxInstances)
                {
                    yield return null;
                    continue;
                }

                yield return Helpers.GetWait(spawnInterval);
                SpawnInstance();
            }
        }

        protected virtual void SpawnInstance()
        {
            Bounds bounds = GameManager.Instance.GameBounds;
            float randX = UnityEngine.Random.Range(bounds.max.x, bounds.min.x);
            float randZ = UnityEngine.Random.Range(bounds.max.z, bounds.min.z);
            var randomPosition = new Vector3(randX, elevation, randZ);
            var newPoint = transform.position + randomPosition;
            T instance = Instantiate(instancePrefab, newPoint, Quaternion.identity);
            instances.Add(instance);
            OnAfterSpawn(instance);
        }

        private void Update()
        {
            float spike = GetDifficultySpike();
            spawnInterval = Mathf.Clamp(1 - spike / 10, minSpawnInterval, maxSpawnInterval);
            currentMaxInstances = Mathf.Clamp((int)(spike * maxInstanceCount), minInstanceCount, maxInstanceCount);
        }

        public virtual float GetDifficultySpike()
        {
            return difficultySpike = GameManager.Instance.GameElapsedTime / spikeFrequency;
        }

        public abstract void OnAfterSpawn(T instance);
    }
}