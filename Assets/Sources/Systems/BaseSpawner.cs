using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class BaseSpawner<T> : Singleton<BaseSpawner<T>>
    where T : MonoBehaviour
{
    bool _isSpawning;
    protected QueuePool<T> instancePool;

    public T prefab;
    public ClampedPrimitive<float> spawnRate;
    public ClampedPrimitive<int> maxInstances;
    public int instanceCount;


    public abstract Vector3 GetRandomPosition();
    protected override void Awake()
    {
        base.Awake();
        instancePool = new QueuePool<T>(prefab, maxInstances.max, transform);
        spawnRate.Clamp();
        maxInstances.Clamp();
        BeginSpawning();
    }
    public void BeginSpawning()
    {
        _isSpawning = true;
        instancePool.Init();
        StartCoroutine(SpawnCoroutine());
    }
    public void StopSpawning()
    {
        _isSpawning = false;
        StopAllCoroutines();
    }
    virtual protected IEnumerator SpawnCoroutine()
    {
        if (!_isSpawning) yield break;

        var wait = Helpers.GetWait(spawnRate);
        while (_isSpawning)
        {
            yield return wait;
            Spawn();
        }
    }
    virtual protected void Spawn()
    {
        instanceCount++;
        var position = GetRandomPosition();
        T instance = instancePool.Get(position, Quaternion.identity);
        OnSpawnedInstance(instance);
    }
    protected void DeSpawn(T instance)
    {
        instanceCount--;
        instancePool.Release(instance);
        OnDesSpawnedInstance(instance);
    }
    virtual protected void OnSpawnedInstance(T instance) { }
    virtual protected void OnDesSpawnedInstance(T instance) { }
}