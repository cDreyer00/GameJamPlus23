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
        var bounds = GameManager.Instance.GameBounds;
        var instance = Instantiate(instancePrefab, pos, Quaternion.identity);
        float rand = UnityEngine.Random.Range(0.6f, 1f);
        float radius = instance.radius = UnityEngine.Random.Range(minRadius, maxRadius) * rand;
        pos.x = Mathf.Clamp(pos.x, bounds.min.x + radius / 2, bounds.max.x - radius / 2);
        pos.z = Mathf.Clamp(pos.z, bounds.min.z + radius / 2, bounds.max.z - radius / 2);

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

        var bounds = GameManager.Instance.GameBounds;
        randomPosition.x = Mathf.Clamp(randomPosition.x, bounds.min.x, bounds.max.x);
        randomPosition.z = Mathf.Clamp(randomPosition.z, bounds.min.z, bounds.max.z);

        return randomPosition;
    }
}