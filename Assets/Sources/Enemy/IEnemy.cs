using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    int Identifier { get; }
    int Health { get; }
    Vector3 Pos { get; }

    void TakeDamage(int damage);

    void SetDestForTimer(Vector3 dest, float timer);
    void SetSpeed(float speed, float timer);

    bool IsDead { get; }

    event Action OnDied;
}