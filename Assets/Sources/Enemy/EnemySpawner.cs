using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    public float difficultySpike;
    
    public int minEnemyCount = 1;
    public int maxEnemyCount = 10;
    public float minSpawnInterval = 0.1f;
    public float maxSpawnInterval = 1f;
    public int minDamage = 1;
    public int maxDamage = 10;

    public override void OnAfterSpawn(EnemyMono instance)
    {
        instance.OnDied += () => instances.Remove(instance);
        instance.target = GameManager.Instance.Player;
        float spike = GetDifficultySpike();
        instance.powerScore = Mathf.Clamp((int)(spike * 10), 1, 10);
        instance.damage = Mathf.Clamp((int)(spike * maxDamage), minDamage, maxDamage);
    }

    public float GetDifficultySpike()
    {
        return difficultySpike = Time.time / 60;
    }

    protected override void Update()
    {
        base.Update();
        float spike = GetDifficultySpike();
        spawnInterval = Mathf.Clamp(1 - spike / 10, minSpawnInterval, maxSpawnInterval);
        maxInstanceCount = Mathf.Clamp((int)(spike * maxEnemyCount), minEnemyCount, maxEnemyCount);
    }
}