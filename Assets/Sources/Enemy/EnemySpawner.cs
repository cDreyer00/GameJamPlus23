using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    [SerializeField] private PlayerController target;
    
    public override void OnAfterSpawn(EnemyMono instance)
    {
        instance.OnDied += () => instances.Remove(instance);
        instance.target = target;
    }
}
