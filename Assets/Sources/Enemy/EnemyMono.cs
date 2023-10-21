using System;
using UnityEngine;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int NextId;
        public int Identifier { get; private set; }

        public int Health
        {
            get => health;
            private set => health = value;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public bool IsDead => Health <= 0;
        public event Action OnDied;

        private void Update()
        {
            if (Health <= 0)
            {
                OnDied?.Invoke();
                //TODO: Maybe pool this object instead of destroying it
                Destroy(gameObject);
            }
        }

        [SerializeField] private int health;

        private void Awake()
        {
            Identifier = NextId++;
        }
    }
}