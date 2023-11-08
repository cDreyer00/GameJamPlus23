using Unity.AI.Navigation;
using UnityEngine;

public class EffectSignSpawner : BaseSpawner<EffectSign>
{
    public NavMeshSurface surface;

    protected void Awake()
    {
        SpawnerSrevice.EffectSignSpawner = this;
    }

    public override Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
    protected override void OnSpawnedInstance(EffectSign instance)
    {
        instance.Pool.onInstanceReleased += OnReleased;

        instance.Init();
    }

    //TODO: temp logic, change later
    public void OnReleased(EffectSign instance)
    {
        instanceCount--;
        OnDesSpawnedInstance(instance);
    }
}