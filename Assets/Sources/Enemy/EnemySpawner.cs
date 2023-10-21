using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    public override void OnAfterSpawn(EnemyMono instance)
    {
        var enemy = instance.GetComponent<IEnemy>();
        enemy.OnDied += () => base.instances.Remove(instance);
    }
}
