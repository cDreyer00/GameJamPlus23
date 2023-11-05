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

[Serializable]
public struct EasingVariable
{
    public float          a;
    public float          b;
    public float          t;
    public EasingFunction function;
    public float Apply(int wave)
    {
        switch (function) {
        case EasingFunction.Linear:
            return Mathf.Lerp(a, b, t * wave);
        case EasingFunction.Exponential:
            return Mathf.Lerp(a, b, MathF.Pow(wave, t));
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}
public sealed class WaveSpawner : BaseSpawner<MonoBehaviour>
{
    public NavMeshSurface surface;
    public int            currentWave;
    public EasingVariable speed;

    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);

    protected override void Spawn()
    {
        var count = maxInstances.Apply(currentWave);
        for (var i = 0; i < count; i++) {
            instanceCount++;
            var position = GetRandomPosition();
            var instance = instancePool.Get(position, Quaternion.identity);
            OnSpawnedInstance(instance);
        }
        StopSpawning();
    }
    protected override void OnSpawnedInstance(MonoBehaviour instance)
    {
        if (instance is Character character) {
            character.Events.onDied += OnEnemyDied;
            var navAgent = character.GetComponent<NavMeshAgent>();
            navAgent.speed = speed.Apply(currentWave);
        }
    }
    protected override void OnDesSpawnedInstance(MonoBehaviour instance)
    {
        if (instanceCount == 0) {
            currentWave++;
            BeginSpawning();
        }
    }
    void OnEnemyDied(ICharacter enemy)
    {
        if (enemy is MonoBehaviour monoBehaviour) {
            DeSpawn(monoBehaviour);
        }
        enemy.Events.onDied -= OnEnemyDied;
    }
}