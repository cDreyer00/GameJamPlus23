using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawner<T> : MonoBehaviour
    where T : MonoBehaviour
{
    protected QueuePool<T> instancePool;

    public bool                    isSpawning;
    public T                       prefab;
    public ClampedPrimitive<float> spawnRate;
    public EasingVariable          maxInstances;
    public int                     instanceCount;

    public abstract Vector3 GetRandomPosition();
    virtual protected void Awake()
    {
        spawnRate.Clamp();
        if (isSpawning) {
            BeginSpawning();
        }
    }
    public void BeginSpawning()
    {
        instancePool ??= new QueuePool<T>(prefab, (int)maxInstances.b, transform);
        isSpawning = true;
        instancePool.Init();
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

        var wait = Helpers.GetWait(spawnRate);
        while (isSpawning) {
            yield return wait;
            Spawn();
        }
    }
    virtual protected void Spawn()
    {
        instanceCount++;
        var position = GetRandomPosition();
        T   instance = instancePool.Get(position, Quaternion.identity);
        OnSpawnedInstance(instance);
    }
    protected void DeSpawn(T instance)
    {
        instanceCount--;
        instancePool.Release(instance);
        OnDesSpawnedInstance(instance);
    }
    virtual protected void OnSpawnedInstance(T instance) {}
    virtual protected void OnDesSpawnedInstance(T instance) {}
}