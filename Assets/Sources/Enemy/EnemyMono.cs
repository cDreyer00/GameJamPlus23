using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using CDreyer;
using UnityEditor;
using UnityEngine.Serialization;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int   nextId;
        public        int   powerScore  = 1;
        public        int   damage      = 1;
        public        float attackDelay = 1f;
        public        float idleTime    = 1;
        float               curDelay;
        bool                canAttack = true;
        public int Identifier { get; private set; }

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AudioClip    damageAudio;
        [SerializeField] private AudioClip    attackAudio;
        [SerializeField] private int          health;

        public IPlayer target;
        public event Action OnDied;

        public Vector3 Pos => transform.position;


        [FormerlySerializedAs("_canMove")] public bool canMove = false;
        public bool CanMove
        {
            set { canMove = value; }
            get { return canMove; }
        }

        [SerializeField] private FeedbackDamage feedback;
        private                  Animator       anim;
        private static readonly  int            AnimIsDead   = Animator.StringToHash("isDead");
        private static readonly  int            AnimIsWalk   = Animator.StringToHash("isWalk");
        private static readonly  int            AnimIsAttack = Animator.StringToHash("isAttack");

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
            else if (!CanMove) {
                CanMove = true;
            }

            if (!canAttack) {
                curDelay += Time.deltaTime;
                if (curDelay >= attackDelay) {
                    canAttack = true;
                    curDelay = 0;
                }
            }

            if (CanMove) {
                SetDestination(target.Pos);
            }
        }

        public void TakeDamage(int damage)
        {
            if (idleTime > 0) return;

            feedback.StartCoroutine(nameof(FeedbackDamage.DamageColor));
            if (damageAudio != null)
                damageAudio.Play();

            Health -= damage;

            if (health <= 0) {
                canAttack = false;
                SetSpeed(0f, 1f);
                anim.SetTrigger(AnimIsDead);
                StartCoroutine(TakeDamageCoroutine());
            }
            else {
                anim.SetTrigger(AnimIsWalk);
            }
        }


        IEnumerator TakeDamageCoroutine()
        {
            yield return Helpers.GetWait(1f);
            if (this.IsDestroyed()) yield break;
            Destroy(gameObject);
            OnDied?.Invoke();
            PowerBar.Instance.UpdatePower(powerScore);
        }

        private IEnumerator Attack(Collision other)
        {
            if (!canAttack) yield break;

            CanMove = false;
            anim.SetBool(AnimIsWalk, false);

            var animClips = anim.GetCurrentAnimatorClipInfo(0);
            var animClip  = animClips[0];
            if (other.gameObject.TryGetComponent<IPlayer>(out var player)) {
                canAttack = false;
                anim.SetTrigger(AnimIsAttack);
                if (animClip.clip.name == "Attack") {
                    player.TakeDamage(damage);
                    CanMove = true;
                    anim.SetBool(AnimIsWalk, true);
                }
            }

            if (attackAudio != null)
                attackAudio.Play();
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
    }
}