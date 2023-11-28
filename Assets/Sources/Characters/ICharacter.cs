using System;
using Sources.Characters.Modules;
using UnityEngine;

public interface ICharacter
{
    Vector3 Position { get; }
    CharacterEvents Events { get; }
}

public class CharacterEvents
{
    public event Action<ICharacter> Died;
    public event Action Initialized;
    public event Action<float> TakeDamage;
    public event Action<float> Freeze;

    public void OnInitialized()
    {
        Initialized?.Invoke();
    }
    public void OnTakeDamage(float amount)
    {
        TakeDamage?.Invoke(amount);
    }
    public void OnFreeze(float duration)
    {
        Freeze?.Invoke(duration);
    }
    public void OnDied(ICharacter character)
    {
        Died?.Invoke(character);
        if (character is Character c) {
            c.Pool.Release(c);
        }
    }
}