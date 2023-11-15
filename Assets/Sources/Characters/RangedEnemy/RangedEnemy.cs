using System;
using System.Linq;
using Sources.Systems;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LookAtCamera))]
[RequireComponent(typeof(FeedbackDamage))]
public class RangedEnemy : Character, IPoolable<Character>
{
    [SerializeField] Projectile projPrefab;
    QueuePool<Projectile>       _projPool;

    public float                                    idleTime;
    public float                                    attackDelay = 1f;
    public NavMeshAgent                             agent;
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
        stateMachine = new StateMachine<FsmCharState, FsmCharEvent>
        {
            currentState = FsmCharState.Idle,

            [FsmCharState.Idle, FsmCharEvent.Chase] = FsmCharState.Chasing,
            [FsmCharState.Idle, FsmCharEvent.Attack] = FsmCharState.Attacking,
            [FsmCharState.Idle, FsmCharEvent.Stop] = FsmCharState.Idle,
            [FsmCharState.Idle, FsmCharEvent.Die] = FsmCharState.Dying,

            [FsmCharState.Chasing, FsmCharEvent.Stop] = FsmCharState.Idle,
            [FsmCharState.Chasing, FsmCharEvent.Attack] = FsmCharState.Attacking,
            [FsmCharState.Chasing, FsmCharEvent.Chase] = FsmCharState.Chasing,
            [FsmCharState.Chasing, FsmCharEvent.Die] = FsmCharState.Dying,

            [FsmCharState.Attacking, FsmCharEvent.Stop] = FsmCharState.Idle,
            [FsmCharState.Attacking, FsmCharEvent.Chase] = FsmCharState.Chasing,
            [FsmCharState.Attacking, FsmCharEvent.Attack] = FsmCharState.Attacking,
            [FsmCharState.Attacking, FsmCharEvent.Die] = FsmCharState.Dying,
        };
        Events.onDied += OnDied;
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
    void OnDied(ICharacter character)
    {
        if (GameManager.IsGameOver) return;
        Helpers.DelayFrames(
            frames: 60,
            action: state => {
                var (pool, self) = state;
                pool.Release(self);
            }, (Pool, this)
        );
    }
    public void OnGet(GenericPool<Character> pool)
    {
        Pool = pool;
        Events.onInitialized?.Invoke();
    }
}