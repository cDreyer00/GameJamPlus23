using Sources.Enemy;
using Sources.Systems;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Environment
{
    public class TrapSpawner : BaseSpawner<TrapMono>
    {
        public NavMeshSurface surface;
        public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
        protected override void OnSpawned(TrapMono instance)
        {
            instance.Init();
            instance.Effect = (Effect)Random.Range(0, 2);
            instance.onTrapDisabled += DeSpawned;
        }
    }
}