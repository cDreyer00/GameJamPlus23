using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Spawner : MonoBehaviour
{
    [SerializeField] BattleConfig _battleConfig;
    [SerializeField] float elapsedTime;

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
    }

    void Update() { _battle.Tick(Time.deltaTime); elapsedTime = _battle.ElapsedTime; }

    void SpawnObj(MonoBehaviour obj, int amount)
    {
        Debug.Log($"spawning {amount} {obj.name}");
    }
}