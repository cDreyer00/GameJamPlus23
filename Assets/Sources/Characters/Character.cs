using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
    [SerializeField] int _health;

    public int Health => _health;
    public bool IsDead => _health <= 0;
    public Vector3 Position => transform.position;

    public event Action<ICharacter> onDied;
    public void Died(ICharacter c) => onDied?.Invoke(this);

    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;

        _health -= amount;
        if (IsDead)
            onDied?.Invoke(this);
    }
}

public interface ICharacter
{
    int Health { get; }
    Vector3 Position { get; }
    bool IsDead { get; }

    void TakeDamage(int amount);
    event Action<ICharacter> onDied;

}