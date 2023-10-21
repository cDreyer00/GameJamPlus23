using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    Vector3 Pos { get; }
    void TakeDamage(int amount);
}