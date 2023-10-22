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

        protected List<T> instances;
        public List<T> Instances => instances;        

        float initTime;

        protected override void Awake()
        {
            base.Awake();
            instances = new List<T>();
            initTime = Time.time;
        }

        protected virtual void Start()
        {
            StartCoroutine(WaitAndSpawn());
        }

        IEnumerator WaitAndSpawn()
        {
            while (true)
            {
                if (instances.Count >= maxInstanceCount)
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
            maxInstanceCount = Mathf.Clamp((int)(spike * maxInstanceCount), minInstanceCount, maxInstanceCount);
        }

        public float GetDifficultySpike()
        {
            return difficultySpike = (Time.time - initTime) / spikeFrequency;
        }

        public abstract void OnAfterSpawn(T instance);
    }
}