using Sources.Enemy;
using Sources.Types;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Systems
{
    public class EnemySpawner : BaseSpawner<EnemyMono>
    {
        public ClampedPrimitive<float> speed;
        public ClampedPrimitive<int>   damage;
        public NavMeshSurface          surface;
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