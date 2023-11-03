using System;
using UnityEngine;
using UnityEngine.AI;

namespace Sources.Characters.MeleeEnemy
{
    public interface IEnemy
    {
        int Health { get; }
        Vector3 Position { get; }
        NavMeshAgent Agent { get; }
        void TakeDamage(int amount);
        void SetDestForTimer(Vector3 dest, float timer);
        void SetSpeed(float speed, float timer);
        bool IsDead { get; }

        event Action<IEnemy> OnDied;
    }
}