using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
    bool _isDead = false;
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public Vector3 Position => transform.position;

    CharacterEvents _events;
    public CharacterEvents Events => _events;

    protected virtual void Awake()
    {
        _events = new();
    }
}