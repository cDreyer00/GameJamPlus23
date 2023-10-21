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
    public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        public int maxInstanceCount = 10;
        public float spawnInterval = 1f;

        public float elevation = 1;

        public T instancePrefab;

        protected List<T> instances;

        protected virtual void Update()
        {
            
        }
        protected virtual void Awake()
        {
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
                if (instances.Count >= maxInstanceCount)
                {
                    yield return null;
                    continue;
                }

                yield return Helpers.GetWait(spawnInterval);
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            int randX = UnityEngine.Random.Range(-10, 10);
            int randZ = UnityEngine.Random.Range(-10, 10);
            var randomPosition = new Vector3(randX, elevation, randZ);
            var newPoint = transform.position + randomPosition;
            T instance = Instantiate(instancePrefab, newPoint, Quaternion.identity);
            instances.Add(instance);
            OnAfterSpawn(instance);
        }

        public abstract void OnAfterSpawn(T instance);
    }
}