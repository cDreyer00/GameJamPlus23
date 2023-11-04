using System;
using System.Linq;
using System.Collections;

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : Character, IPoolable<MeleeEnemy>
{
    public int damage = 1;
    public float attackDelay = 1f;
    public float idleTime = 1;

    float _lastAttackTime;
    bool _canAttack = true;
    Animator _anim;


    [SerializeField] private AudioClip damageAudio;
    [SerializeField] private AudioClip attackAudio;
    [SerializeField] private FeedbackDamage feedback;

    public bool canMove;
    public Transform target;

    public GenericPool<MeleeEnemy> Pool { get; set; }

    private readonly static int AnimIsDead = Animator.StringToHash("isDead");
    private readonly static int AnimIsWalk = Animator.StringToHash("isWalk");
    private readonly static int AnimIsAttack = Animator.StringToHash("isAttack");

    protected override void Awake()
    {
        base.Awake();
    }

    void OnEnable()
    {
        Events.onTakeDamage += OnTakeDamage;
        Events.onDied += OnDied;
    }

    void OnDisable()
    {
        Events.onTakeDamage -= OnTakeDamage;
        Events.onDied -= OnDied;

        var sprite = feedback.GetComponent<SpriteRenderer>();
        if (!ReferenceEquals(sprite, null))
        {
            sprite.color = Color.white;
        }
    }

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        target = GameManager.Instance.Player.transform;

        NavMeshMovement movement = Modules.OfType<NavMeshMovement>().FirstOrDefault();
        if(movement != null)
            movement.Target = target;
    }

    private void Update()
    {
        if (GameManager.IsGameOver) return;

        idleTime -= Time.deltaTime;
        if (idleTime > 0) return;
        canMove = true;

        if (!_canAttack)
        {
            _lastAttackTime += Time.deltaTime;
            if (_lastAttackTime >= attackDelay)
            {
                _canAttack = true;
                _lastAttackTime = 0;
            }
        }
    }

    void OnTakeDamage(float amount)
    {
        if (IsDead) return;
        if (idleTime > 0) return;

        feedback.StartCoroutine(nameof(FeedbackDamage.DamageColor));
        if (damageAudio != null)
            damageAudio.Play();

        _anim.SetTrigger(AnimIsWalk);
    }

    void OnDied()
    {
        _canAttack = false;
        _anim.SetTrigger(AnimIsDead);
        Helpers.Delay(0.8f, () => { Pool.Release(this); });
    }

    private IEnumerator Attack(Collision other)
    {
        if (!_canAttack) yield break;
        canMove = false;
        _anim.SetBool(AnimIsWalk, false);

        var animClips = _anim.GetCurrentAnimatorClipInfo(0);
        var animClip = animClips[0];
        if (other.gameObject.TryGetComponent<PlayerController>(out var player))
        {
            _canAttack = false;
            _anim.SetTrigger(AnimIsAttack);
            if (animClip.clip.name == "Attack")
            {
                player.Events.onTakeDamage?.Invoke(damage);
                canMove = true;
                _anim.SetBool(AnimIsWalk, true);
            }
        }

        if (attackAudio) attackAudio.Play();

        canMove = true;
    }

    private void OnCollisionStay(Collision other) => StartCoroutine(Attack(other));
    private void OnCollisionEnter(Collision collision) => StartCoroutine(Attack(collision));

    public void OnGet(GenericPool<MeleeEnemy> pool)
    {
        this.Pool = pool;
        Events.onInitialized?.Invoke();
    }

    public void OnRelease()
    {
    }

    public void OnCreated()
    {
    }
}