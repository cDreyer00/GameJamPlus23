using Sources.Systems;
using Unity.AI.Navigation;
using UnityEngine;

namespace Sources.Effects
{
    public class EffectSignSpawner : BaseSpawner<EffectSign>
    {
        public NavMeshSurface surface;
        public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
        protected override void OnSpawnedInstance(EffectSign instance)
        {
            instance.Init();
        }
    }
}