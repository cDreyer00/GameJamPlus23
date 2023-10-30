using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

namespace Sources.Environment
{
    public class TrapSpawner : RampingSpawner<TrapMono>
    {
        public override Vector3 GetRandomPosition() => surface.GetRandomPoint();
        protected override void OnSpawned(TrapMono instance)
        {
            instance.Init();
            instance.Effect = (Effect)Random.Range(0, 2);
            instance.onTrapDisabled += DeSpawned;
        }
    }
}