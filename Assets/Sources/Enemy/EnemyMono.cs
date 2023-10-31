using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using CDreyer;
using Sources.Player;
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
        float               _curDelay;
        bool                _canAttack = true;
        public int Identifier { get; private set; }

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private AudioClip    damageAudio;
        [SerializeField] private AudioClip    attackAudio;
        [SerializeField] private int          health;

        public IPlayer target;
        public event Action<EnemyMono> OnDied;

        public Vector3 Pos => transform.position;


        public bool canMove;
        public bool CanMove
        {
            set => canMove = value;
            get => canMove;
        }

        [SerializeField] private FeedbackDamage feedback;
        private                  Animator       _anim;
        private readonly static  int            AnimIsDead   = Animator.StringToHash("isDead");
        private readonly static  int            AnimIsWalk   = Animator.StringToHash("isWalk");
        private readonly static  int            AnimIsAttack = Animator.StringToHash("isAttack");

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
            _anim = GetComponentInChildren<Animator>();
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

            if (!_canAttack) {
                _curDelay += Time.deltaTime;
                if (_curDelay >= attackDelay) {
                    _canAttack = true;
                    _curDelay = 0;
                }
            }

            if (CanMove) {
                SetDestination(target.Position);
            }
        }

        public void TakeDamage(int amount)
        {
            if (idleTime > 0) return;

            feedback.StartCoroutine(nameof(FeedbackDamage.DamageColor));
            if (damageAudio != null)
                damageAudio.Play();

            Health -= amount;

            if (health <= 0) {
                _canAttack = false;
                SetSpeed(0f, 1f);
                _anim.SetTrigger(AnimIsDead);
                Helpers.ActionCallbackCr(() => OnDied?.Invoke(this), 1f);
            }
            else {
                _anim.SetTrigger(AnimIsWalk);
            }
        }
        private IEnumerator Attack(Collision other)
        {
            if (!_canAttack) yield break;

            CanMove = false;
            _anim.SetBool(AnimIsWalk, false);

            var animClips = _anim.GetCurrentAnimatorClipInfo(0);
            var animClip  = animClips[0];
            if (other.gameObject.TryGetComponent<IPlayer>(out var player)) {
                _canAttack = false;
                _anim.SetTrigger(AnimIsAttack);
                if (animClip.clip.name == "Attack") {
                    player.TakeDamage(damage);
                    CanMove = true;
                    _anim.SetBool(AnimIsWalk, true);
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
        void OnDisable()
        {
            var sprite = feedback.GetComponent<SpriteRenderer>();
            if (!ReferenceEquals(sprite, null)) {
                sprite.color = Color.white;
            }
        }
    }
}