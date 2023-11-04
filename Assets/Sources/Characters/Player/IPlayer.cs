using Sources.Camera;
using UnityEngine;

public interface IPlayer : ICharacter
{
    float CurDelay { get; }
    float ShootDelay { get; }
    public CameraShake Came { get; }
}