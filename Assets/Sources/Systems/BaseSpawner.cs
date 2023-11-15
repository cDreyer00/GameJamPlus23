using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour
{
    protected QueuePool<T> instancePool;

    [Header("References")]
    public T prefab;
    public NavMeshSurface navMeshSurface;

    [Header("Curves")]
    public AnimationCurve speed;
    public AnimationCurve maxInstances;
    public AnimationCurve spawnRate;

    [Header("Values")]
    public int waveNumber;
    public bool isSpawning;
    public int  instanceCount;
    public virtual Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(navMeshSurface.navMeshData.sourceBounds);

    void Start()
    {
        if (isSpawning) {
            BeginSpawning();
        }
    }
    public void BeginSpawning()
    {
        instancePool ??= new QueuePool<T>(prefab, (int)maxInstances.Evaluate(1), transform);
        if (!instancePool.Initialized) {
            instancePool.Init();
            instancePool.onInstanceReleased += DeSpawn;
        }
        isSpawning = true;
        StartCoroutine(SpawnCoroutine());
    }
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }
    virtual protected IEnumerator SpawnCoroutine()
    {
        if (!isSpawning) yield break;

        var wait = Helpers.GetWait(spawnRate.Evaluate(waveNumber));
        while (isSpawning) {
            yield return wait;
            Spawn();
        }
    }
    public void Spawn()
    {
        var count = Mathf.RoundToInt(maxInstances.Evaluate(waveNumber));
        for (var i = 0; i < count; i++) {
            instanceCount++;
            var position = GetSpawnPosition();
            var instance = instancePool.Get(position, Quaternion.identity);
            if (instance is Character character && character.TryGetModule<NavMeshMovement>(out var navMeshMovement)) {
                navMeshMovement.Agent.speed = speed.Evaluate(waveNumber);
            }
            OnSpawnedInstance(instance);
        }
        StopSpawning();
    }
    public void DeSpawn(T instance)
    {
        if (instanceCount-- == 0) {
            waveNumber++;
            BeginSpawning();
        }
        OnDesSpawnedInstance(instance);
    }
    virtual protected void OnSpawnedInstance(T instance) {}
    virtual protected void OnDesSpawnedInstance(T instance) {}
}