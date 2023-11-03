using UnityEngine;
using Unity.AI.Navigation;

public class MeleeEnemySpawner : BaseSpawner<MeleeEnemy>
{
    public ClampedPrimitive<float> speed;
    public ClampedPrimitive<int> damage;
    public NavMeshSurface surface;
    protected override void Awake()
    {
        base.Awake();
        speed.Clamp();
        damage.Clamp();
    }
    public override Vector3 GetRandomPosition() => NavMeshRandom.InsideBounds(surface.navMeshData.sourceBounds);
    protected override void OnSpawnedInstance(MeleeEnemy instance)
    {
        instance.OnDied += OnEnemyDied;
        instance.damage = damage;
        var agent = instance.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = speed.Value = speed;
    }
    void OnEnemyDied(IEnemy meleeEnemy)
    {
        DeSpawn((MeleeEnemy)meleeEnemy);
        meleeEnemy.OnDied -= OnEnemyDied;
    }
}