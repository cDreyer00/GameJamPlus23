using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
    HashSet<CharacterModule> _modules = new();
    CharacterEvents _events = new();
    bool _isDead = false;

    public bool IsDead { get => _isDead; set => _isDead = value; }
    public Vector3 Position => transform.position;
    public CharacterEvents Events => _events;
    public IEnumerable<CharacterModule> Modules => _modules;

    protected virtual void Awake()
    {

    }

    public void AddModule(CharacterModule module)
    {
        _modules.Add(module);
    }

    public void RemoveModule(CharacterModule module)
    {
        _modules.Remove(module);
    }
}