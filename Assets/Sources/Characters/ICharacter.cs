using System;
using UnityEngine;

public interface ICharacter
{
    Vector3 Position { get; }
    bool IsDead { get; }
    CharacterEvents Events { get; }
}

public class CharacterEvents
{
    public Action             onInitialized;
    public Action<float>      onTakeDamage;
    public Action<ICharacter> onDied;
}