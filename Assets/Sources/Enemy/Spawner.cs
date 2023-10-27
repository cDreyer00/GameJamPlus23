using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CDreyer;
using Sources.Types;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = System.Random;

namespace Sources.Enemy
{
    public abstract class Spawner<T> : CDreyer.Singleton<Spawner<T>> where T : MonoBehaviour
    {
        [SerializeField] float difficultySpike;

        public int   startSpawnTimer;
        public float spikePeriod = 60f;
        public float elevation   = 1;

        public ClampedPrimitive<int>   damage;
        public ClampedPrimitive<float> spawnInterval;
        public ClampedPrimitive<int>   maxInstances;

        public T instancePrefab;

        protected List<T> instances = new();
        public List<T> Instances => instances;

        protected override void Awake()
        {
            base.Awake();
            instances = new List<T>();
        }

        virtual protected void Start()
        {
            StartCoroutine(WaitAndSpawn());
        }

        IEnumerator WaitAndSpawn()
        {
            while (true) {
                if (GameManager.Instance.GameElapsedTime < startSpawnTimer) {
                    yield return null;
                    continue;
                }

                if (instances.Count >= maxInstances) {
                    yield return null;
                    continue;
                }

                yield return Helpers.GetWait(spawnInterval);
                SpawnInstance();
            }
        }

        virtual protected void SpawnInstance()
        {
            Bounds bounds         = GameManager.Instance.GameBounds;
            float  randX          = UnityEngine.Random.Range(bounds.max.x, bounds.min.x);
            float  randZ          = UnityEngine.Random.Range(bounds.max.z, bounds.min.z);
            var    randomPosition = new Vector3(randX, elevation, randZ);
            var    newPoint       = transform.position + randomPosition;
            T      instance       = Instantiate(instancePrefab, newPoint, Quaternion.identity);
            instances.Add(instance);
            OnAfterSpawn(instance);
        }

        private void Update()
        {
            float spike = GetDifficultySpike();
            spawnInterval.Value = 1 - spike / 10;
            maxInstances.Value = (int)Mathf.Lerp(maxInstances.Value, maxInstances.max, spike / 10);
        }

        public virtual float GetDifficultySpike()
        {
            return difficultySpike = GameManager.Instance.GameElapsedTime / spikePeriod;
        }

        public abstract void OnAfterSpawn(T instance);
    }
}