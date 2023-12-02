using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

public class Battle
{
    public float ElapsedTime { get; private set; }
    public bool IsCompleted { get; private set; }
    public Wave CurWave { get; private set; }

    BattleConfig _config;
    Queue<Wave> _wavesQueue = new();

    public event Action onBattleEnd;

    public event Action<MonoBehaviour, int> onUpdateSpawnCount;

    public bool IsPaused { get; set; }

    public Battle(BattleConfig config)
    {
        _config = config;

        foreach (var wc in _config.Waves)
            _wavesQueue.Enqueue(new(this, wc));
    }

    public void StartWave()
    {
        if (CurWave == null || CurWave.IsCompleted)
            GetNextWave();

        IsPaused = false;
    }

    public void Tick(float deltaTime)
    {
        if (IsCompleted || IsPaused) return;

        ElapsedTime += deltaTime;
        CurWave.Tick(deltaTime);
    }

    void CompleteWave()
    {
        if (IsCompleted || CurWave == null) return;
        IsPaused = true;

        if (CurWave != null)
            CurWave.onWaveComplete -= CompleteWave;

        if (_wavesQueue.Count == 0)
        {
            IsCompleted = true;
            onBattleEnd?.Invoke();
            return;
        }
    }

    void GetNextWave()
    {
        CurWave = _wavesQueue.Dequeue();
        CurWave.onWaveComplete += CompleteWave;
    }

    public void SpawnObjs(MonoBehaviour obj, int amount)
    {
        onUpdateSpawnCount?.Invoke(obj, amount);
    }
}

public class Wave
{
    public float ElapsedTime { get; private set; }
    public readonly float duration;

    Battle _battle;
    WaveConfig _config;
    int[] _objsCounter;
    public bool IsCompleted { get; private set; }

    public event Action onWaveComplete;

    public Wave(Battle battle, WaveConfig config)
    {
        _battle = battle;
        _config = config;
        duration = config.Duration;
        _objsCounter = new int[config.ObjectsList.Length];
    }

    public void Tick(float deltaTime)
    {
        if (IsCompleted)
            return;

        ElapsedTime += deltaTime;

        if (ElapsedTime >= duration)
        {
            IsCompleted = true;
            onWaveComplete?.Invoke();
        }

        UpdateSpawns();
    }

    void UpdateSpawns()
    {
        var objs = _config.ObjectsList;
        for (int i = 0; i < objs.Length; i++)
        {
            int amount = objs[i].GetAmount(ElapsedTime);
            var counter = _objsCounter[i];
            if (counter >= amount) continue;

            int diff = amount - counter;
            ObjectCurveConfig objConfig = objs[i];
            _battle.SpawnObjs(objConfig.Obj, diff);
            _objsCounter[i] = amount;
        }
    }
}