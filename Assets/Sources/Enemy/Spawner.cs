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
    public abstract class Spawner : MonoBehaviour
    {
        public int maxInstanceCount = 10;
        public float spawnInterval = 1f;

        public float elevation = 1;

        public GameObject instancePrefab;

        protected List<GameObject> instances;

        private void Awake()
        {
            instances = new List<GameObject>();
        }

        private void Start()
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
            var instance = Instantiate(instancePrefab, newPoint, Quaternion.identity);
            instances.Add(instance);
            OnAfterSpawn(instance);
        }

        public abstract void OnAfterSpawn(GameObject instance);
    }
}