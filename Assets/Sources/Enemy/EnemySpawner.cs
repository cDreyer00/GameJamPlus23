using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    private float _timeElapsed;
    public float difficultySpike;

    [SerializeField] private PlayerController target;

    public override void OnAfterSpawn(EnemyMono instance)
    {
        instance.OnDied += () => instances.Remove(instance);
        instance.target = target;
        float spike = GetDifficultySpike();
        instance.powerScore = Mathf.Clamp((int)(spike * 10), 1, 10);
        instance.damage = Mathf.Clamp((int)(spike * 10), 1, 10);
    }

    public float GetDifficultySpike()
    {
        return difficultySpike = _timeElapsed / 60;
    }

    protected override void Update()
    {
        base.Update();
        _timeElapsed += Time.deltaTime;
        float spike = GetDifficultySpike();
        spawnInterval = Mathf.Clamp(1 - spike / 10, 0.1f, 1);
        maxInstanceCount = Mathf.Clamp((int)(spike * 10), 1, 10);
    }
}