using Sources.Enemy;
using Sources.Systems;
using Sources.Types;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Environment
{
    public class EnvAttackSpawner : BaseSpawner<EnvAreaAttack>
    {
        Animator _animator;

        [SerializeField] float          maxDistFromPlayer;
        [SerializeField] float          closePlayerChance = 0.4f;

        public float                 radius = 1f;
        public ClampedPrimitive<int> damage;
        protected override void OnSpawned(EnvAreaAttack instance)
        {
            instance.OnExplode += DeSpawned;
            var   pos            = instance.transform.position;
            var   bounds         = GameManager.Instance.GameBounds;
            float instanceRadius = instance.radius = radius;
            pos.x = Mathf.Clamp(pos.x, bounds.min.x + instanceRadius / 2, bounds.max.x - instanceRadius / 2);
            pos.z = Mathf.Clamp(pos.z, bounds.min.z + instanceRadius / 2, bounds.max.z - instanceRadius / 2);
            instance.transform.position = pos;
            instance.Init();
            instance.damage = damage;
        }
        public override Vector3 GetRandomPosition()
        {
            float rand = Random.Range(0f, 1f);

            if (rand < closePlayerChance) {
                float randX = Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
                float randZ = Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
                NavMesh.SamplePosition(new Vector3(randX, 0, randZ), out var hit, 10, NavMesh.AllAreas);

                float elevation      = hit.position.y;
                var   randomPosition = GameManager.Instance.Player.Position + new Vector3(randX, elevation, randZ);

                var bounds = GameManager.Instance.GameBounds;
                randomPosition.x = Mathf.Clamp(randomPosition.x, bounds.min.x, bounds.max.x);
                randomPosition.z = Mathf.Clamp(randomPosition.z, bounds.min.z, bounds.max.z);

                return randomPosition;
            }
            else {
                var   bounds = GameManager.Instance.GameBounds;
                float randX  = Random.Range(bounds.min.x, bounds.max.x);
                float randZ  = Random.Range(bounds.min.z, bounds.max.z);
                NavMesh.SamplePosition(new Vector3(randX, 0, randZ), out var hit, 10, NavMesh.AllAreas);
                float elevation      = hit.position.y;
                var   randomPosition = new Vector3(randX, elevation, randZ);

                return randomPosition;
            }
        }
    }
}