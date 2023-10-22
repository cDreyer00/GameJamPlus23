using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using CDreyer;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int nextId;
        public int powerScore = 1;
        public int damage = 1;
        public float attackDelay = 1f;
        public float idleTime = 1;
        float curDelay;
        bool canAttack = true;
        public int Identifier { get; private set; }

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] AudioClip damageAudio;
        [SerializeField] AudioClip attackAudio;

        public IPlayer target;
        public event Action OnDied;

        public Vector3 Pos => transform.position;

        [SerializeField] private int health;

        public bool _canMove = false;
        public bool canMove
        {
            get
            {
                anim.SetBool(AnimIsWalk, _canMove);
                return _canMove;
            }
        }

        [SerializeField] FeedbackDamage feedback;
        private Animator anim;
        private static readonly int AnimIsDead = Animator.StringToHash("isDead");
        private static readonly int AnimIsWalk = Animator.StringToHash("isWalk");
        private static readonly int AnimIsAttack = Animator.StringToHash("isAttack");

        public int Health
        {
            get => health;
            private set => health = value;
        }

        public bool IsDead => Health <= 0;

        private void Start()
        {
            Identifier = nextId++;
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (GameManager.IsGameOver)
                return;
            
            idleTime -= Time.deltaTime;
            if (idleTime > 0) return;
            else if (!canMove)
                _canMove = true;

            if (!canAttack)
            {
                curDelay += Time.deltaTime;
                if (curDelay >= attackDelay)
                {
                    canAttack = true;
                    curDelay = 0;
                }
            }

            if (canMove)
            {
                SetDestination(target.Pos);
            }
        }

        public void TakeDamage(int damage)
        {
            if (idleTime > 0) return;

            feedback.StartCoroutine("DamageColor");
            if (damageAudio != null)
                damageAudio.Play();

            Health -= damage;

            if (health <= 0)
            {
                canAttack = false;
                SetSpeed(0f, 1f);
                anim.SetTrigger(AnimIsDead);
                Helpers.ActionCallback(() =>
                {
                    if (this.IsDestroyed()) return;
                    Destroy(gameObject);
                    OnDied?.Invoke();
                    PowerBar.Instance.UpdatePower(powerScore);
                }, 1f);
            }
            else
            {
                anim.SetTrigger(AnimIsWalk);
            }
        }

        private void Attack(Collision other)
        {
            if (!canAttack) return;
            _canMove = false;

            if (other.gameObject.TryGetComponent<IPlayer>(out var player))
            {
                anim.SetTrigger(AnimIsAttack);
                Helpers.ActionCallback(() =>
                {
                    player.TakeDamage(damage);
                    _canMove = true;
                }, 0.34f);
            }

            if (attackAudio != null)
                attackAudio.Play();

            canAttack = false;
        }

        private void OnCollisionStay(Collision other) => Attack(other);
        private void OnCollisionEnter(Collision collision) => Attack(collision);

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
            Helpers.ActionCallback(() =>
            {
                if (this.IsDestroyed()) return;
                agent.speed = normalSpeed;
            }, timer);
        }
    }
}