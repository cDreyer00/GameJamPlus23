using System;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int nextId;
        public int powerScore = 1;
        public int damage = 1;
        public float attackDelay = 1f;
        float curDelay;
        bool canAttack = true;
        float idleTime = 0.75f;
        public int Identifier { get; private set; }

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] AudioClip damageAudio;
        [SerializeField] AudioClip attackAudio;

        Animator animator;


        public IPlayer target;
        public event Action OnDied;

        [SerializeField] private int health;

        public bool canMove = true;
        private static readonly int AnimIsAttack = Animator.StringToHash("isAttack");
        private static readonly int AnimIsDead = Animator.StringToHash("isDead");
        private static readonly int AnimIsWalk = Animator.StringToHash("isWalk");

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
            animator = GetComponentInChildren<Animator>();

            animator.SetBool(AnimIsWalk, true);
        }

        private void Update()
        {
            idleTime -= Time.deltaTime;
            if (idleTime > 0) return;

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
            if (damageAudio != null)
                damageAudio.Play();

            Health -= damage;

            if (health <= 0)
            {
                agent.isStopped = true;
                animator.SetTrigger(AnimIsDead);
                Helpers.ActionCallback(() =>
                {
                    Destroy(gameObject);
                    OnDied?.Invoke();
                    PowerBar.Instance.UpdatePower(powerScore);
                }, 1f);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (!canAttack) return;

            var player = other.gameObject.GetComponent<IPlayer>();

            if (player != null)
            {
                animator.SetTrigger(AnimIsAttack);
                player.TakeDamage(damage);
            }

            if (attackAudio != null)
                attackAudio.Play();

            canAttack = false;
        }

        public void SetDestination(Vector3 position)
        {
            agent.SetDestination(position);
        }

        public float Speed
        {
            get => agent.speed;
            set => agent.speed = value;
        }
    }
}