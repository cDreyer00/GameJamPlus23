using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.AI.Navigation;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour
{
    [SerializeField] BattleConfig _battleConfig;
    [SerializeField] float elapsedTime;
    [SerializeField] NavMeshSurface navMeshSurface;

    Dictionary<GameObject, QueuePool<MonoBehaviour>> poolsDict = new();
    List<MonoBehaviour> spawnedInstances = new();
    Battle _battle;

    public event Action onAllEnemiesDead;

    void Awake()
    {
        GlobalInstancesBehaviour.GlobalInstances.AddInstance("spawner", this);
    }

    void Start()
    {
        Init(_battleConfig);
        StartWave();
    }

    void Update() { _battle.Tick(Time.deltaTime); }

    public void Init(BattleConfig battleConfig)
    {
        _battleConfig = battleConfig;
        _battle = new(battleConfig);
        _battle.onUpdateSpawnCount += SpawnObj;

        // initialize pools
        var wavesObjs = battleConfig.GetObjects();
        foreach (var objs in wavesObjs)
            foreach (var obj in objs)
            {
                if (poolsDict.ContainsKey(obj.gameObject)) continue;
                var pool = new QueuePool<MonoBehaviour>(obj, 15, transform);
                pool.Init();
                poolsDict.Add(obj.gameObject, pool);
            }
    }

    public Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(navMeshSurface.navMeshData.sourceBounds);

    public void StartWave()
    {
        _battle.StartWave();
        Debug.Log("new wave started");
    }

    public T[] GetInstancesByTag<T>(string tag) where T : MonoBehaviour
    {
        var targets = spawnedInstances.Where(i =>
        {
            if (i.IsDestroyed() || !i.gameObject.activeInHierarchy)
            {
                spawnedInstances.Remove(i);
                return false;
            }

            return i.CompareTag(tag);
        }).ToArray();

        List<T> values = new();
        foreach (var t in targets)
            if (t.TryGetComponent(out T type))
                values.Add(type);

        return values.ToArray();
    }

    void SpawnObj(MonoBehaviour obj, int amount)
    {
        if (!poolsDict.TryGetValue(obj.gameObject, out var pool))
        {
            Debug.LogError("pool not found");
            return;
        }

        for (; amount > 0; amount--)
        {
            var i = pool.Get(GetSpawnPosition(), Quaternion.identity);
            spawnedInstances.Add(i);
            pool.onInstanceReleased += OnInstanceRelease;
        }
    }

    void OnInstanceRelease(MonoBehaviour instance)
    {
        if (!spawnedInstances.Contains(instance)) return;
        spawnedInstances.Remove(instance);

        bool containsEnemy = spawnedInstances.Any(i => i.CompareTag("Enemy"));
        if (_battle.CurWave.IsCompleted && !containsEnemy)
        {
            onAllEnemiesDead?.Invoke();
        }
    }
}