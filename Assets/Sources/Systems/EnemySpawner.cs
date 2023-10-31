using Sources.Enemy;
using Sources.Types;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Systems
{
    public sealed class EnemySpawner : BaseSpawner<EnemyMono>
    {
        public           ClampedPrimitive<float> speed;
        public           ClampedPrimitive<int>   damage;
        public           NavMeshSurface          surface;
        [SerializeField] WaveManager             wave;
        protected override void Awake()
        {
            base.Awake();
            speed.Clamp();
            damage.Clamp();
        }

        public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
        protected override void OnSpawned(EnemyMono instance)
        {
            instance.OnDied += OnEnemyDied;
            instance.target = GameManager.PlayerHealthBar.Player;
            instance.powerScore = 10;
            instance.damage = damage;
            var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
            speed.Value = speed;
            agent.speed = speed;
        }
        void OnEnemyDied(EnemyMono enemy)
        {
            DeSpawned(enemy);
            enemy.OnDied -= OnEnemyDied;
        }
    }
}