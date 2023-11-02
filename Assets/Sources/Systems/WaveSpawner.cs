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
        public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);

        protected override void Spawn()
        {
            maxInstances.Value = MeleeEnemyCount(currentWave);
            for (var i = 0; i < maxInstances; i++) {
                instanceCount++;
                var position = GetRandomPosition();
                var instance = instancePool.Get(position, Quaternion.identity);
                OnSpawnedInstance(instance);
            }
            StopSpawning();
        }
        protected override void OnSpawnedInstance(MonoBehaviour instance)
        {
            if (instance is IEnemy enemy) {
                enemy.OnDied += OnEnemyDied;
                enemy.Agent.speed = speed.Value = MeleeEnemySpeed(currentWave);
            }
        }
        protected override void OnDesSpawnedInstance(MonoBehaviour instance)
        {
            if (instanceCount == 0) {
                currentWave++;
                BeginSpawning();
            }
        }
        void OnEnemyDied(IEnemy enemy)
        {
            if (enemy is MonoBehaviour monoBehaviour) {
                DeSpawn(monoBehaviour);
            }
            enemy.OnDied -= OnEnemyDied;
        }

        static int MeleeEnemyCount(int wave)
        {
            const float Base = 2;
            return (int)MathF.Pow(Base, wave) + 1;
        }
        static float MeleeEnemySpeed(int wave)
        {
            const float Base = 1.3f;
            return MathF.Pow(Base, wave) + 1;
        }
    }
}