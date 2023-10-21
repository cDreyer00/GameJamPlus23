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

        [SerializeField] MeshRenderer mr;

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
            float randX = UnityEngine.Random.Range(mr.bounds.max.x, mr.bounds.min.x);
            float randZ = UnityEngine.Random.Range(mr.bounds.max.z, mr.bounds.min.z);
            var randomPosition = new Vector3(randX, elevation, randZ);
            var newPoint = transform.position + randomPosition;
            T instance = Instantiate(instancePrefab, newPoint, Quaternion.identity);
            instances.Add(instance);
            OnAfterSpawn(instance);
        }

        protected virtual void Update()
        {

        }

        public abstract void OnAfterSpawn(T instance);
    }
}