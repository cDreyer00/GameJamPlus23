using System;
using System.Collections;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    public int damage = 1;
    public float attackDelay = 1f;
    public float idleTime = 1;

    float _lastAttackTime;
    bool _canAttack = true;
    Animator _anim;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private AudioClip damageAudio;
    [SerializeField] private AudioClip attackAudio;
    [SerializeField] private int health;
    [SerializeField] private FeedbackDamage feedback;

    public bool canMove;
    public IPlayer target;
    public event Action<IEnemy> OnDied;
    public NavMeshAgent Agent => agent;
    public Vector3 Position => transform.position;


    private readonly static int AnimIsDead = Animator.StringToHash("isDead");
    private readonly static int AnimIsWalk = Animator.StringToHash("isWalk");
    private readonly static int AnimIsAttack = Animator.StringToHash("isAttack");

    public bool IsDead => health <= 0;
    public int Health => health;

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

        if (!_canAttack)
        {
            _lastAttackTime += Time.deltaTime;
            if (_lastAttackTime >= attackDelay)
            {
                _canAttack = true;
                _lastAttackTime = 0;
            }
        }

        if (canMove)
        {
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

        if (health <= 0)
        {
            _canAttack = false;
            SetSpeed(0f, 1f);
            _anim.SetTrigger(AnimIsDead);
            Helpers.Delay(1f, () => OnDied?.Invoke(this));
        }
        else
        {
            _anim.SetTrigger(AnimIsWalk);
        }
    }
    private IEnumerator Attack(Collision other)
    {
        if (!_canAttack) yield break;

        canMove = false;
        _anim.SetBool(AnimIsWalk, false);

        var animClips = _anim.GetCurrentAnimatorClipInfo(0);
        var animClip = animClips[0];
        if (other.gameObject.TryGetComponent<IPlayer>(out var player))
        {
            _canAttack = false;
            _anim.SetTrigger(AnimIsAttack);
            if (animClip.clip.name == "Attack")
            {
                player.TakeDamage(damage);
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
        if (!ReferenceEquals(sprite, null))
        {
            sprite.color = Color.white;
        }
    }
}