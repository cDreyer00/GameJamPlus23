using UnityEngine;

namespace Sources.Enemy
{
    public interface IEnemy
    {
        int Identifier { get; }
        int Health { get; }
        Vector3 Pos { get; }

        void TakeDamage(int amount);

        void SetDestForTimer(Vector3 dest, float timer);
        void SetSpeed(float speed, float timer);

        bool IsDead { get; }
    }
}