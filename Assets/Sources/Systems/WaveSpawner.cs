using System;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.Serialization;


public enum EasingFunction
{
    Linear,
    Exponential,
}

[Serializable]
public struct EasingVariable
{
    public float t;
    public ClampedPrimitive<float> value;
    public EasingFunction function;
    public float Apply(int wave)
    {
        switch (function)
        {
            case EasingFunction.Linear:
                return Mathf.Lerp(value.min, value.max, t * wave);
            case EasingFunction.Exponential:
                return Mathf.Lerp(value.min, value.max, Mathf.Pow(t, wave));
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
public sealed class WaveSpawner : BaseSpawner<MonoBehaviour>
{
    public NavMeshSurface surface;
    public int currentWave;
    public EasingVariable maxInstanceScaling;
    public EasingVariable speed;

    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);

    protected override void Spawn()
    {
        maxInstances.Value = (int)maxInstanceScaling.Apply(currentWave);
        for (var i = 0; i < maxInstances; i++)
        {
            instanceCount++;
            var position = GetRandomPosition();
            var instance = instancePool.Get(position, Quaternion.identity);
            OnSpawnedInstance(instance);
        }
        StopSpawning();
    }
    protected override void OnSpawnedInstance(MonoBehaviour instance)
    {
        if (instance is MeleeEnemy enemy)
        {
            enemy.Pool.onInstanceReleased += OnEnemyDied;
            // enemy.Agent.speed = speed.Apply(currentWave);
        }
    }
    protected override void OnDesSpawnedInstance(MonoBehaviour instance)
    {
        if (instanceCount == 0)
        {
            currentWave++;
            BeginSpawning();
        }
    }
    void OnEnemyDied(ICharacter enemy)
    {
        instanceCount--;
        if (enemy is MonoBehaviour monoBehaviour)
            OnDesSpawnedInstance(monoBehaviour);
    }
}