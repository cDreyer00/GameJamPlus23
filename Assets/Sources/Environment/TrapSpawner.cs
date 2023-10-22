using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

namespace Sources.Environment
{
    public class TrapSpawner : Spawner<TrapMono>
    {
        [SerializeField] private Effect effect;

        private Dictionary<Effect, IEnumerator> _effects;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnAfterSpawn(TrapMono instance)
        {
            instance.Init();
            instance.onTrapDisabled += trap =>
            {
                instances.Remove(trap);
            };
        }

        protected override void SpawnInstance()
        {
            base.SpawnInstance();
            var instance = instances[^1];
            var pos = instance.transform.position;
            var bounds = GameManager.Instance.GameBounds;
            float radius = instance.Radius;
            pos.x = Mathf.Clamp(pos.x, bounds.min.x + radius, bounds.max.x - radius);
        }
    }
}