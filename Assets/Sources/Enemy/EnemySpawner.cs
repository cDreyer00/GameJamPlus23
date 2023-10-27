using Sources.Types;
using UnityEngine;

namespace Sources.Enemy
{
    public sealed class EnemySpawner : Spawner<EnemyMono>
    {
        public ClampedPrimitive<float> speed;
        public override void OnAfterSpawn(EnemyMono instance)
        {
            instance.OnDied += () => instances.Remove(instance);
            instance.target = GameManager.Instance.Player;
            float spike = GetDifficultySpike();
            instance.powerScore = Mathf.Clamp((int)(spike * 10), 1, 10);
            instance.damage = (int)(spike * damage);
            var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
            speed.Value = spike * speed;
            agent.speed = speed;
        }
    }
}