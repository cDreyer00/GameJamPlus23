using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using Sources.Enemy;
using UnityEngine;

public class EnvAttackSpawner : Spawner<EnvAreaAttack>
{
    [SerializeField] float maxDistFromPlayer;
    
    public float minRadius = 1f;
    public float maxRadius = 10f;

    protected override void SpawnInstance()
    {
        Vector3 pos = GetRandomPosition();
        var bounds = mr.bounds;
        var instance = Instantiate(instancePrefab, pos, Quaternion.identity);
        var radius = instance.radius;
        pos.x = Mathf.Clamp(pos.x, bounds.min.x + radius/2, bounds.max.x - radius/2);
        pos.z = Mathf.Clamp(pos.z, bounds.min.z + radius/2, bounds.max.z - radius/2);
        instance.transform.position = pos;
        instances.Add(instance);
        OnAfterSpawn(instance);
    }

    public override void OnAfterSpawn(EnvAreaAttack instance)
    {
        instance.Init();
        float spike = GetDifficultySpike();
        float rand = UnityEngine.Random.Range(0.5f, 1f);
        instance.damage = Mathf.Clamp((int)(spike * maxDamage), minDamage, maxDamage);
        instance.onExplode += (i) => instances.Remove(i);
    }

    Vector3 GetRandomPosition()
    {
        float randX = UnityEngine.Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
        float randZ = UnityEngine.Random.Range(-maxDistFromPlayer, maxDistFromPlayer);
        var randomPosition = GameManager.Instance.Player.Pos + new Vector3(randX, elevation, randZ);
        randomPosition.x = Mathf.Clamp(randomPosition.x, mr.bounds.min.x, mr.bounds.max.x);
        randomPosition.z = Mathf.Clamp(randomPosition.z, mr.bounds.min.z, mr.bounds.max.z);

        return randomPosition;
    }
}