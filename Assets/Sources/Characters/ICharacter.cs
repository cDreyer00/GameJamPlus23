using System;
using Sources.Characters.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ICharacter
{
    Vector3 Position { get; }
    CharacterEvents Events { get; }
}

public class CharacterEvents
{
    public event Action<ICharacter> OnDied;
    public event Action OnInitialized;
    public event Action<float> OnTakeDamage;
    public event Action<float> OnFreeze;

    public void Initialized()
    {
        OnInitialized?.Invoke();
    }
    public void TakeDamage(float amount)
    {
        OnTakeDamage?.Invoke(amount);
    }
    public void Freeze(float duration)
    {
        OnFreeze?.Invoke(duration);
    }
    public void Died(ICharacter character)
    {
        OnDied?.Invoke(character);
        if (character is Character c) {
            if (c.Pool == null) {
                Object.Destroy(c.gameObject);
            }
            else {
                c.Pool.Release(c);
            }
        }
    }
}