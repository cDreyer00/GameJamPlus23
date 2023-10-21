using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public sealed class EnemySpawner : Spawner<EnemyMono>
{
    public override void OnAfterSpawn(EnemyMono instance)
    {
        instance.OnDied += () => instances.Remove(instance);
        instance.target = GameManager.Instance.Player;
    }

    public void SetSpawnRate()
    {

    }
}
