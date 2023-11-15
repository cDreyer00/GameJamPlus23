using UnityEngine;
using Unity.AI.Navigation;

public sealed class WaveSpawner : BaseSpawner<Character>
{
    public AnimationCurve speed;
    public int            currentWave;

    protected override void Spawn()
    {
        var count = Mathf.RoundToInt(maxInstances.Evaluate(currentWave));
        for (var i = 0; i < count; i++) {
            instanceCount++;
            var position = GetSpawnPosition();
            var instance = instancePool.Get(position, Quaternion.identity);
            if (instance.TryGetModule<NavMeshMovement>(out var navMeshMovement)) {
                navMeshMovement.Agent.speed = speed.Evaluate(currentWave);
            }
            OnSpawnedInstance(instance);
        }
        StopSpawning();
    }
    protected override void OnDesSpawnedInstance(Character instance)
    {
        if (instanceCount == 0) {
            currentWave++;
            BeginSpawning();
        }
    }
}