using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class MeleeEnemySpawner : BaseSpawner<MeleeEnemy>
{
    public ClampedPrimitive<float> speed;
    public ClampedPrimitive<int>   damage;
    List<MeleeEnemy>               _enemies;
    protected void Awake()
    {
        speed.Clamp();
        damage.Clamp();
        _enemies = new();
        SpawnerSrevice.MeleeEnemySpawner = this;
    }
    public override Vector3 GetSpawnPosition() => NavMeshRandom.InsideBounds(navMeshSurface.navMeshData.sourceBounds);
    protected override void OnSpawnedInstance(MeleeEnemy instance)
    {
        instancePool.onInstanceReleased += OnReleased;
        instance.damage = damage;
        var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed.Value = speed;

        _enemies.Add(instance);
    }

    protected override void OnDesSpawnedInstance(MeleeEnemy instance)
    {
        _enemies.Remove(instance);
    }

    public IEnumerable<MeleeEnemy> GetAllEnemies()
    {
        return _enemies;
    }

    //TODO: temp logic, change later
    public void OnReleased(MeleeEnemy instance)
    {
        instanceCount--;
        OnDesSpawnedInstance(instance);
    }
}