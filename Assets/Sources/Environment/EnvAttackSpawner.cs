using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using Sources.Enemy;
using UnityEngine;

public class EnvAttackSpawner : Spawner<EnvAreaAttack>
{
    private Vector3[] randomPositions;

    protected override void Awake()
    {
        base.Awake();
        randomPositions = new Vector3[10];
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

    void GetRandomPositions(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = GetRandomPosition();
        }
    }

    protected override void SpawnInstance()
    {
        float closeToPLayerBias = 0.7f;
        float rand = Random.Range(0f, 1f);
        GetRandomPositions(randomPositions);
        Vector3 vec;
        if (rand < closeToPLayerBias)
        {
            var closest = randomPositions
                .OrderBy(p => Vector3.Distance(p, GameManager.Instance.Player.Pos))
                .First();
            vec = closest;
        }
        else
        {
            var first = randomPositions.First();
            vec = first;
        }
        
        var instance = Instantiate(instancePrefab, vec, Quaternion.identity);
        instances.Add(instance);
        OnAfterSpawn(instance);
    }
}