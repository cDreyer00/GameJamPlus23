using System;
using System.Linq;
using Sources.Systems;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LookAtCamera))]
public class RangedEnemy : Character, IPoolable<Character>
{
    [SerializeField] Projectile projPrefab;
    QueuePool<Projectile>       _projPool;

    public float                                      idleTime;
    public float                                      attackDelay = 1f;
    public NavMeshAgent                               agent;
    public StateMachine<FsmCharState, FsmCharEvent> stateMachine;

    public GenericPool<Character> Pool { get; set; }
    public NavMeshAgent Agent => agent;
    public override string Team => "Enemy";

    Vector3 _lastPlayerPos;
    Vector3 _playerDelta;

    float _curAttackDelay;
    void Awake()
    {
        _projPool = new QueuePool<Projectile>(projPrefab, 10, transform);
        _projPool.Init();
        stateMachine = StateMachine<FsmCharState, FsmCharEvent>.FullyConnectedGraph;
    }
    void Update()
    {
        if (GameManager.IsGameOver) return;
        idleTime -= Time.deltaTime;
        if (idleTime > 0) return;

        _curAttackDelay += Time.deltaTime;
        if (_curAttackDelay >= attackDelay) {
            stateMachine.MoveNext(FsmCharEvent.Attack);
            _curAttackDelay = 0;
            Shoot();
        }

        var playerPos = GameManager.Instance.Player.Position;
        _playerDelta = (playerPos - _lastPlayerPos) * Time.deltaTime;
        _lastPlayerPos = playerPos;
    }
    public void Shoot()
    {
        var t         = transform;
        var position  = t.position;
        var player    = GameManager.Instance.Player;
        var playerPos = player.Position;
        var playerDir = Quaternion.LookRotation(player.Position - position);
        var proj      = _projPool.Get(position, playerDir);
        proj.target = playerPos + _playerDelta;
        proj.IgnoreTeam(Team);
    }
    public void OnGet(GenericPool<Character> pool)
    {
        Pool = pool;
        Events.onInitialized?.Invoke();
    }
}