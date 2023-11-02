using Sources.Types;
using UnityEngine;

namespace Sources.Enemy
{
    public sealed class EnemyNavSurfaceSpawner : RampingSpawner<EnemyMono>
    {
        public ClampedPrimitive<float> speed;
        public ClampedPrimitive<int>   damage;
        public override Vector3 GetRandomPosition() => surface.GetRandomPoint();
        protected override void OnSpawned(EnemyMono instance)
        {
            instance.OnDied += OnEnemyDied;
            instance.target = GameManager.Instance.Player;
            float difficultyLevel = DifficultyMod;
            instance.powerScore = Mathf.Clamp((int)(difficultyLevel * 10), 1, 10);
            instance.damage = (int)(difficultyLevel * damage);
            var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
            speed.Value = difficultyLevel * speed;
            agent.speed = speed;
        }
        void OnEnemyDied(EnemyMono enemy)
        {
            DeSpawned(enemy);
            enemy.OnDied -= OnEnemyDied;
        }
    }
}