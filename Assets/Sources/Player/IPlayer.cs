using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    Vector3 Position { get; }
    float CurDelay { get; }
    float ShootDelay { get; }
    public CameraShake Came {get;}    

    void TakeDamage(int amount);
}