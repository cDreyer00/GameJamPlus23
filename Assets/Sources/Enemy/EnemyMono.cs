using System;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int nextId;
        public int powerScore = 1;
        public int Identifier { get; private set; }

        [SerializeField] private NavMeshAgent agent;

        public IPlayer target;
        public event Action OnDied;

        [SerializeField] private int health;

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
        }

        private void Update()
        {
            agent.SetDestination(target.Pos);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (health <= 0)
            {
                Destroy(gameObject);
                OnDied?.Invoke();
                PowerBar.Instance.UpdatePower(powerScore);
            }
        }
    }
}