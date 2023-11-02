using System;
using System.Collections;
using Sources.Enemy;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Systems
{
    public sealed class WaveSpawner : EnemySpawner
    {
        public int currentWave;
        public static int EnemyCount(int wave)
        {
            const float BaseCount  = 3;
            const float Multiplier = 1.33f;

            return (int)(BaseCount * MathF.Pow(Multiplier, wave)) + 1;
        }
        public static int EnemySpeed(int wave)
        {
            const float BaseSpeed  = 3;
            const float Multiplier = 1.1f;

            return (int)(BaseSpeed * MathF.Pow(Multiplier, wave));
        }
        public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);

        protected override IEnumerator SpawnCoroutine()
        {
            if (cancellationRequested) yield break;
            yield return Helpers.GetWait(startSpawnTimer);

            var wait = Helpers.GetWait(spawnRate);
            while (!cancellationRequested) {
                if (activeInstances >= maxInstances) {
                    yield return null;
                    continue;
                }
                yield return wait;
                var enemies = EnemyCount(currentWave);
                for (int i = 0; i < enemies; i++) {
                    Spawned();
                }
            }
        }

        protected override void OnSpawned(EnemyMono instance)
        {
            instance.OnDied += OnEnemyDied;
            instance.target = GameManager.Instance.Player;
            instance.powerScore = 10;
            instance.damage = damage;
            var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.speed = speed.Value = speed;
        }

        void OnEnemyDied(EnemyMono enemy)
        {
            DeSpawned(enemy);
            enemy.OnDied -= OnEnemyDied;
        }
    }
}