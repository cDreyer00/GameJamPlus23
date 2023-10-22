using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using Sources.Enemy;
using UnityEngine;

public class EnvAttackSpawner : Spawner<EnvAreaAttack>
{
    [SerializeField] float maxDistFromPlayer;
    [SerializeField] Vector3 minBounds;
    [SerializeField] Vector3 maxBounds;

    protected override void SpawnInstance()
    {
        Vector3 pos = GetRandomPosition();
        var instance = Instantiate(instancePrefab, pos, Quaternion.identity);
        instances.Add(instance);
        OnAfterSpawn(instance);
    }

    public override void OnAfterSpawn(EnvAreaAttack instance)
    {
        instance.Init();
        instance.onExplode += (i) => instances.Remove(i);
    }

    Vector3 GetRandomPosition()
    {
        minBounds = mr.bounds.min;
        maxBounds = mr.bounds.max;

        float randX = UnityEngine.Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
        float randZ = UnityEngine.Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
        var randomPosition = GameManager.Instance.Player.Pos + new Vector3(randX, elevation, randZ);
        randomPosition.x = Mathf.Clamp(randomPosition.x, mr.bounds.min.x, mr.bounds.max.x);
        randomPosition.z = Mathf.Clamp(randomPosition.z, mr.bounds.min.z, mr.bounds.max.z);

        return randomPosition;
    }
}