using System;
using System.Collections;
using Sources.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LookAtCamera))]
public class MeleeEnemy : Character
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
    [SerializeField] private int            health;
    [SerializeField] private FeedbackDamage feedback;

    public bool canMove;
    public NavMeshAgent Agent => agent;
    public override string Team => "Enemy";
    public ICharacter target;

    private readonly static int AnimIsDead   = Animator.StringToHash("isDead");
    private readonly static int AnimIsWalk   = Animator.StringToHash("isWalk");
    private readonly static int AnimIsAttack = Animator.StringToHash("isAttack");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        target = GameManager.Instance.Player;
    }

    private void Update()
    {
        if (GameManager.IsGameOver) return;

        idleTime -= Time.deltaTime;
        if (idleTime > 0) return;
        canMove = true;

        if (!_canAttack) {
            _lastAttackTime += Time.deltaTime;
            if (_lastAttackTime >= attackDelay) {
                _canAttack = true;
                _lastAttackTime = 0;
            }
        }

        if (canMove) {
            SetDestination(target.Position);
        }
    }

    public void TakeDamage(int amount)
    {
        if (idleTime > 0) return;

        feedback.StartCoroutine(nameof(FeedbackDamage.DamageColor));
        if (damageAudio != null)
            damageAudio.Play();

        health -= amount;

        if (health <= 0) {
            _canAttack = false;
            SetSpeed(0f, 1f);
            _anim.SetTrigger(AnimIsDead);
            Helpers.Delay(1f, () => Events.onDied(this));
        }
        else {
            _anim.SetTrigger(AnimIsWalk);
        }
    }
    private IEnumerator Attack(Collision other)
    {
        if (!_canAttack) yield break;

        canMove = false;
        _anim.SetBool(AnimIsWalk, false);

        var animClips = _anim.GetCurrentAnimatorClipInfo(0);
        var animClip  = animClips[0];
        if (other.gameObject.TryGetComponent<Character>(out var player)) {
            _canAttack = false;
            _anim.SetTrigger(AnimIsAttack);
            if (animClip.clip.name == "Attack") {
                player.Events.onTakeDamage(damage);
                canMove = true;
                _anim.SetBool(AnimIsWalk, true);
            }
        }

        if (attackAudio) attackAudio.Play();

        canMove = true;
    }

    private void OnCollisionStay(Collision other) => StartCoroutine(Attack(other));
    private void OnCollisionEnter(Collision collision) => StartCoroutine(Attack(collision));

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void SetDestForTimer(Vector3 dest, float timer)
    {
        idleTime = timer;
        SetDestination(dest);
    }

    public void SetSpeed(float speed, float timer)
    {
        float normalSpeed = agent.speed;
        agent.speed = speed;
        StartCoroutine(SetSpeedCoroutine(normalSpeed, timer));
    }
    IEnumerator SetSpeedCoroutine(float speed, float timer)
    {
        yield return Helpers.GetWait(timer);
        if (this.IsDestroyed()) yield break;
        agent.speed = speed;
    }
    void OnDisable()
    {
        var sprite = feedback.GetComponent<SpriteRenderer>();
        if (!ReferenceEquals(sprite, null)) {
            sprite.color = Color.white;
        }
    }
}