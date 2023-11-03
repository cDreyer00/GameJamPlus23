using Unity.AI.Navigation;
using UnityEngine;

public class EffectSignSpawner : BaseSpawner<EffectSign>
{
    public NavMeshSurface surface;
    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
    protected override void OnSpawnedInstance(EffectSign instance)
    {
        instance.Pool.onInstanceReleased += OnReleased;

        instance.Init();

        SpawnerSrevice.EffecSignSpawner = this;
    }

    //TODO: temp logic, change later
    public void OnReleased(EffectSign instance)
    {
        instanceCount--;
        OnDesSpawnedInstance(instance);
    }
}