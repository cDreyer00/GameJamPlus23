using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

public abstract class BaseSpawner<T> : MonoBehaviour
    where T : MonoBehaviour
{
    protected QueuePool<T> instancePool;

    public bool                    isSpawning;
    public T                       prefab;
    public ClampedPrimitive<float> spawnRate;
    public AnimationCurve          maxInstances;
    public int                     instanceCount;
    public NavMeshSurface          navMeshSurface;
    public virtual Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(navMeshSurface.navMeshData.sourceBounds);

    void Start()
    {
        spawnRate.Clamp();
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

        var wait = Helpers.GetWait(spawnRate);
        while (isSpawning) {
            yield return wait;
            Spawn();
        }
    }
    virtual protected void Spawn()
    {
        instanceCount++;
        T instance = instancePool.Get(GetSpawnPosition(), Quaternion.identity);
        OnSpawnedInstance(instance);
    }
    virtual protected void DeSpawn(T instance)
    {
        instanceCount--;
        OnDesSpawnedInstance(instance);
    }
    virtual protected void OnSpawnedInstance(T instance) {}
    virtual protected void OnDesSpawnedInstance(T instance) {}
}