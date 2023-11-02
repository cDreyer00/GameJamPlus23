using System;
using System.Collections;
using Sources.cdreyer;
using Sources.Characters.Player;
using Sources.SoundManager;
using Sources.Systems;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Characters.MeleeEnemy
{
    public class MeleeEnemy : MonoBehaviour, IEnemy
    {
        public NavMeshAgent agent;
        public int          damage      = 1;
        public float        attackDelay = 1f;
        public float        idleTime    = 1;

        float _lastAttackTime;
        bool  _canAttack = true;

        [SerializeField] private AudioClip      damageAudio;
        [SerializeField] private AudioClip      attackAudio;
        [SerializeField] private int            health;
        [SerializeField] private FeedbackDamage feedback;

        public IPlayer target;
        public event Action<IEnemy> OnDied;
        public NavMeshAgent Agent => agent;

        public Vector3 Position => transform.position;


        [SerializeField] bool canMove;
        public bool CanMove
        {
            get => canMove;
            set => canMove = value;
        }

        private Animator _anim;

        private readonly static int AnimIsDead   = Animator.StringToHash("isDead");
        private readonly static int AnimIsWalk   = Animator.StringToHash("isWalk");
        private readonly static int AnimIsAttack = Animator.StringToHash("isAttack");

        public int Health
        {
            get => health;
            private set => health = value;
        }

        public bool IsDead => Health <= 0;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            _anim = GetComponentInChildren<Animator>();
            target = GameManager.GameManager.Instance.Player;
        }

        private void Update()
        {
            if (GameManager.GameManager.IsGameOver) return;

            idleTime -= Time.deltaTime;
            if (idleTime > 0) {
                return;
            }
            CanMove = true;

            if (!_canAttack) {
                _lastAttackTime += Time.deltaTime;
                if (_lastAttackTime >= attackDelay) {
                    _canAttack = true;
                    _lastAttackTime = 0;
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

            canMove = false;
            _anim.SetBool(AnimIsWalk, false);

            var animClips = _anim.GetCurrentAnimatorClipInfo(0);
            var animClip  = animClips[0];
            if (other.gameObject.TryGetComponent<IPlayer>(out var player)) {
                _canAttack = false;
                _anim.SetTrigger(AnimIsAttack);
                if (animClip.clip.name == "Attack") {
                    player.TakeDamage(damage);
                    canMove = true;
                    _anim.SetBool(AnimIsWalk, true);
                }
            }

            if (attackAudio != null)
                attackAudio.Play();

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
}