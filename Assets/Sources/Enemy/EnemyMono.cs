using System;
using UnityEngine;

namespace Sources.Enemy
{
    public class EnemyMono : MonoBehaviour, IEnemy
    {
        public static int NextId;
        public int Identifier { get; private set; }

        public event Action OnDied;

        [SerializeField] private int health;
        public int Health
        {
            get => health;
            private set => health = value;
        }
        public bool IsDead => Health <= 0;

        private void Awake()
        {
            Identifier = NextId++;
        }


        public void TakeDamage(int damage)
        {
            Health -= damage;

            if (health <= 0)
            {
                Destroy(gameObject);
                OnDied?.Invoke();
            }
        }
    }
}