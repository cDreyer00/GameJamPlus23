using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using Sources.Enemy;
using UnityEngine;

public class EnvAttackSpawner : Spawner<EnvAreaAttack>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnAfterSpawn(EnvAreaAttack instance)
    {
        instance.Init();
        instance.onExplode += (i) => instances.Remove(i);
    }

    Vector3 GetRandomPosition()
    {
        float randX = UnityEngine.Random.Range(mr.bounds.max.x, mr.bounds.min.x);
        float randZ = UnityEngine.Random.Range(mr.bounds.max.z, mr.bounds.min.z);
        var randomPosition = new Vector3(randX, elevation, randZ);
        return randomPosition;
    }

    Vector3 GetRandomPositionAroundOrigin(Vector3 origin, float radius)
    {
        var randomPosition = Random.insideUnitSphere * radius;
        randomPosition += origin;
        randomPosition.y = elevation;
        return randomPosition;
    }
    protected override void SpawnInstance()
    {
        var vec = GetRandomPositionAroundOrigin(transform.position, 10f);
        var instance = Instantiate(instancePrefab, vec, Quaternion.identity);
        instances.Add(instance);
        OnAfterSpawn(instance);
    }
}