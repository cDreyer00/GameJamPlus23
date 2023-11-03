using Sources.Camera;
using UnityEngine;

public interface IPlayer
{
    Vector3 Position { get; }
    float CurDelay { get; }
    float ShootDelay { get; }
    public CameraShake Came { get; }
    void TakeDamage(int amount);
}