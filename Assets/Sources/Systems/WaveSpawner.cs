using System;
using Sources.Characters.MeleeEnemy;
using Sources.Types;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Systems
{
    public sealed class WaveSpawner : BaseSpawner<MonoBehaviour>
    {
        public ClampedPrimitive<float> speed;
        public NavMeshSurface          surface;
        public int                     currentWave;
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

        protected override void Spawn()
        {
            var enemyCount = EnemyCount(currentWave);
            for (var i = 0; i < enemyCount; i++) {
                var position = GetRandomPosition();
                var instance = instancePool.Get(position, Quaternion.identity);
                OnSpawnedInstance(instance);
            }
        }
        protected override void OnSpawnedInstance(MonoBehaviour instance)
        {
            if (instance is IEnemy enemy) {
                enemy.OnDied += OnEnemyDied;
                var agent = enemy.Agent;
                agent.speed = speed.Value = EnemySpeed(currentWave);
            }
        }
        void OnEnemyDied(IEnemy meleeEnemy)
        {
            if (meleeEnemy is MonoBehaviour monoBehaviour) {
                DeSpawn(monoBehaviour);
            }
            meleeEnemy.OnDied -= OnEnemyDied;
        }
    }
}