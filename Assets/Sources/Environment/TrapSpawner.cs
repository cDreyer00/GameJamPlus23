using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public class TrapSpawner : Spawner<TrapMono>
{
    public override void OnAfterSpawn(TrapMono instance)
    {
        instance.TrapDirection = (Direction)Random.Range(0, 4);
        instance.OnTrapTriggered += trap =>
        {
            instances.Remove(trap);
            Destroy(trap.gameObject);
        };
    }
}
