using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.AI.Navigation;
using Mono.Cecil.Cil;

public class Spawner : MonoBehaviour
{
    [SerializeField] BattleConfig _battleConfig;
    [SerializeField] float elapsedTime;
    [SerializeField] NavMeshSurface navMeshSurface;

    Dictionary<GameObject, QueuePool<MonoBehaviour>> poolsDict = new();

    Battle _battle;

    void Start()
    {
        Init(_battleConfig);
    }

    public void Init(BattleConfig battleConfig)
    {
        _battleConfig = battleConfig;
        _battle = new(battleConfig);
        _battle.onUpdateSpawnCount += SpawnObj;

        // initialize pool
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

    //                                                      to debug
    void Update() { _battle.Tick(Time.deltaTime); elapsedTime = _battle.ElapsedTime; }

    public virtual Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(navMeshSurface.navMeshData.sourceBounds);

    void SpawnObj(MonoBehaviour obj, int amount)
    {        
        if (!poolsDict.TryGetValue(obj.gameObject, out var pool))
        {
            Debug.LogError("pool not found");
            return;
        }

        for (; amount > 0; amount--)
        {
            pool.Get(GetSpawnPosition(), Quaternion.identity);
        }
    }
}