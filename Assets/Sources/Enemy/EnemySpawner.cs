using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CDreyer;
using Unity.VisualScripting;
using Random = System.Random;

namespace Sources.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        public int maxEnemyCount = 10;
        public float spawnInterval = 1f;

        public float elevation = 1;

        public GameObject enemyPrefab;

        private List<GameObject> _enemies;

        private void Awake()
        {
            _enemies = new List<GameObject>();
        }

        private void Start()
        {
            StartCoroutine(WaitAndSpawn());
        }

        IEnumerator WaitAndSpawn()
        {
            while (true)
            {
                if (_enemies.Count >= maxEnemyCount)
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
            var enemy = Instantiate(enemyPrefab, newPoint, Quaternion.identity);
            _enemies.Add(enemy);

            var enemyMono = enemy.GetComponent<EnemyMono>();
            enemyMono.OnDied += () => { _enemies.Remove(enemy); };
        }
    }
}