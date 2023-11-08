using System;
using System.Collections;
using Sources.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LookAtCamera))]
[RequireComponent(typeof(FeedbackDamage))]
public class MeleeEnemy : Character, IPoolable<Character>
{
    public int   damage      = 1;
    public float attackDelay = 1f;
    public float idleTime    = 1;

    float    _lastAttackTime;
    bool     _canAttack = true;
    Animator _anim;

    [SerializeField] private NavMeshAgent   agent;
    [SerializeField] private AudioClip      damageAudio;
    [SerializeField] private AudioClip      attackAudio;
    [SerializeField] private FeedbackDamage feedback;
    public NavMeshAgent Agent => agent;
    public override string Team => "Enemy";

    private readonly static int AnimIsDead   = Animator.StringToHash("isDead");
    private readonly static int AnimIsWalk   = Animator.StringToHash("isWalk");
    private readonly static int AnimIsAttack = Animator.StringToHash("isAttack");
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
    }
    private void Start()
    {
        if (TryGetModule<NavMeshMovement>(out var meshMovement)) {
            meshMovement.Target = GameManager.Instance.Player.transform;
        }
        _anim = GetComponentInChildren<Animator>();
        Events.onTakeDamage += OnTakeDamage;
        Events.onDied += OnDied;
    }
    private void Update()
    {
        if (GameManager.IsGameOver) return;

        idleTime -= Time.deltaTime;
        if (idleTime <= 0) {
            stateMachine.MoveNext(FsmCharEvent.Chase);
        }

        if (!_canAttack) {
            _lastAttackTime += Time.deltaTime;
            if (_lastAttackTime >= attackDelay) {
                _canAttack = true;
                _lastAttackTime = 0;
            }
        }
    }
    void OnDied(ICharacter character)
    {
        if (GameManager.IsGameOver) return;
        _anim.SetBool(AnimIsDead, true);
        Helpers.DelayFrames(
            frames: 60,
            action: static state => {
                var (pool, self) = state;
                pool.Release(self);
            }, (Pool, this)
        );
    }
    void OnTakeDamage(float amount)
    {
        if (GameManager.IsGameOver) return;
        if (idleTime > 0) return;

        feedback.StartCoroutine(nameof(FeedbackDamage.DamageColor));

        if (damageAudio) damageAudio.Play();

        _anim.SetTrigger(AnimIsWalk);
    }
    private IEnumerator Attack(Collision other)
    {
        if (stateMachine.currentState != FsmCharState.Attacking) {
            yield break;
        }

        _anim.SetBool(AnimIsWalk, false);

        var animClips = _anim.GetCurrentAnimatorClipInfo(0);
        var animClip  = animClips[0];
        if (other.gameObject.TryGetComponent<Character>(out var player)) {
            _canAttack = false;
            _anim.SetTrigger(AnimIsAttack);
            if (animClip.clip.name == "Attack") {
                player.Events.onTakeDamage(damage);
                _anim.SetBool(AnimIsWalk, true);
            }
        }

        if (attackAudio) attackAudio.Play();

        if (stateMachine.currentState is FsmCharState.Attacking) {
            stateMachine.MoveNext(FsmCharEvent.Chase);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (_canAttack) {
            stateMachine.MoveNext(FsmCharEvent.Attack);
            StartCoroutine(Attack(collision));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (_canAttack) {
            stateMachine.MoveNext(FsmCharEvent.Attack);
            StartCoroutine(Attack(collision));
        }
    }
    public void OnRelease()
    {
        var sprite = feedback.GetComponent<SpriteRenderer>();
        if (!ReferenceEquals(sprite, null)) {
            sprite.color = Color.white;
        }
    }
    public GenericPool<Character> Pool { get; set; }
    public void OnGet(GenericPool<Character> pool)
    {
        Pool = pool;
        Events.onInitialized.Invoke();
    }
}