using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    int Identifier { get; }
    int Health { get; }

    void TakeDamage(int damage);

    bool IsDead { get; }

    event Action OnDied;
}