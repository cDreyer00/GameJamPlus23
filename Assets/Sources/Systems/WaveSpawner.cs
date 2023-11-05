using System;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum EasingFunction
{
    Linear,
    Exponential,
}
public sealed class WaveSpawner : BaseSpawner<Character>
{
    public AnimationCurve speed;
    public NavMeshSurface surface;
    public int            currentWave;

    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);

    protected override void Spawn()
    {
        var count = Mathf.RoundToInt(maxInstances.Evaluate(currentWave));
        for (var i = 0; i < count; i++) {
            instanceCount++;
            var position = GetRandomPosition();
            var instance = instancePool.Get(position, Quaternion.identity);
            OnSpawnedInstance(instance);
        }
        StopSpawning();
    }
    protected override void OnSpawnedInstance(Character character)
    {
        character.Events.onDied += OnEnemyDied;
        var navAgent = character.GetComponent<NavMeshAgent>();
        navAgent.speed = speed.Evaluate(currentWave);
    }
    protected override void OnDesSpawnedInstance(Character instance)
    {
        if (instanceCount == 0) {
            currentWave++;
            BeginSpawning();
        }
    }
    void OnEnemyDied(ICharacter character)
    {
        DeSpawn((Character)character);
        character.Events.onDied -= OnEnemyDied;
    }
}