using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class MeleeEnemySpawner : BaseSpawner<MeleeEnemy>
{
    public ClampedPrimitive<float> speed;
    public ClampedPrimitive<int> damage;
    public NavMeshSurface surface;
    List<MeleeEnemy> _enemies;
    protected override void Awake()
    {
        base.Awake();

        speed.Clamp();
        damage.Clamp();
        _enemies = new();

        SpawnerSrevice.MeleeEnemySpawner = this;
    }
    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
    protected override void OnSpawnedInstance(MeleeEnemy instance)
    {
        instance.onDied += OnEnemyDied;
        instance.damage = damage;
        var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed.Value = speed;

        _enemies.Add(instance);
    }

    protected override void OnDesSpawnedInstance(MeleeEnemy instance)
    {
        _enemies.Remove(instance);
    }

    void OnEnemyDied(ICharacter meleeEnemy)
    {
        DeSpawn((MeleeEnemy)meleeEnemy);
        meleeEnemy.onDied -= OnEnemyDied;
    }

    public IEnumerable<IEnemy> GetAllEnemies()
    {
        return _enemies;
    }
}