using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    public float minSpeed = 1f;
    public float maxSpeed = 10f;

    public override void OnAfterSpawn(EnemyMono instance)
    {
        instance.OnDied += () => instances.Remove(instance);
        instance.target = GameManager.Instance.Player;
        float spike = GetDifficultySpike();
        instance.powerScore = Mathf.Clamp((int)(spike * 10), 1, 10);
        instance.damage = Mathf.Clamp((int)(spike * maxDamage), minDamage, maxDamage);
        var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = Mathf.Clamp(spike * maxSpeed, minSpeed, maxSpeed);
    }
}  