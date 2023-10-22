using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    Vector3 Pos { get; }
    float CurDelay { get; }
    float ShootDelay { get; }

    void TakeDamage(int amount);
}