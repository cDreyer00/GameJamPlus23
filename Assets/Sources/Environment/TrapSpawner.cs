using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

namespace Sources.Environment
{
    public class TrapSpawner : Spawner<TrapMono>
    {
        [SerializeField] private Effect effect;

        [SerializeField] private EffectManager effectManager;

        private Dictionary<Effect, IEnumerator> _effects;

        protected override void Awake()
        {
            base.Awake();
            _effects = new Dictionary<Effect, IEnumerator>
            {
                [Effect.Confusion] = effectManager.Confusion(3f),
            };
        }

        public override void OnAfterSpawn(TrapMono instance)
        {
            instance.TrapDirection = (Direction)Random.Range(0, 4);
            instance.onTrapTriggered += trap =>
            {
                instances.Remove(trap);
                Destroy(trap.gameObject);
                StartCoroutine(_effects[effect]);
            };
        }

        protected override void SpawnInstance()
        {
            base.SpawnInstance();
            var instance = instances[^1];
            var pos = instance.transform.position;
            var bounds = mr.bounds;
            float radius = instance.radius;
            pos.x = Mathf.Clamp(pos.x, bounds.min.x + radius, bounds.max.x - radius);
        }
    }
}