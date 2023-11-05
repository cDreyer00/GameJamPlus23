using UnityEngine;
using UnityEngine.AI;
//[RequireComponent(typeof(LookAt))]
public class RangedEnemy : Character
{
    [SerializeField] Projectile projPrefab;

    QueuePool<Projectile> _projPool;

    public float        idleTime;
    public float        attackDelay = 1f;
    public NavMeshAgent agent;
    public StateMachine stateMachine;
    public NavMeshAgent Agent => agent;
    public override string Team => "Enemy";

    float _curAttackDelay;
    void Awake()
    {
        _projPool = new QueuePool<Projectile>(projPrefab, 10, transform);
        _projPool.Init();
        stateMachine = StateMachine.FullyConnectedGraph;
    }
    void Update()
    {
        if (GameManager.IsGameOver) return;
        idleTime -= Time.deltaTime;
        if (idleTime > 0) return;

        _curAttackDelay += Time.deltaTime;
        if (_curAttackDelay >= attackDelay) {
            stateMachine.MoveNext(EventVariable.Attack);
            _curAttackDelay = 0;
            Shoot();
        }
    }
    public void SetDestForTimer(Vector3 dest, float timer)
    {
        idleTime = timer;
        stateMachine.MoveNext(EventVariable.Stop);
        agent.SetDestination(dest);
    }
    public void SetSpeed(float speed, float timer)
    {
        float normalSpeed = agent.speed;
        agent.speed = speed;
        Helpers.Delay(timer, () => agent.speed = normalSpeed);
    }
    public void Shoot()
    {
        var t         = transform;
        var position  = t.position;
        var player    = GameManager.Instance.Player;
        var playerDir = Quaternion.LookRotation(player.Position - position);
        var proj      = _projPool.Get(position, playerDir);
        if (!proj.ignoreList.Contains(Team)) {
            proj.ignoreList.Add(Team);
        }
    }
}