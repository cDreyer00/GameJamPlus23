using System;
using System.Collections;
using UnityEngine;

namespace Sources.Systems
{
    public class SpawnerAggregate : MonoBehaviour
    {
        public BaseSpawner<MonoBehaviour>[] spawners;
        void Awake()
        {
            BeginSpawning();
        }
        public void BeginSpawning()
        {
            foreach (var spawner in spawners) {
                spawner.BeginSpawning();
            }
        }
        public void StopSpawning()
        {
            foreach (var spawner in spawners) {
                spawner.StopSpawning();
            }
        }
    }
}