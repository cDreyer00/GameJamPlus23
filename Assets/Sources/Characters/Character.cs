using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour, ICharacter, IPoolable<MonoBehaviour>
{
    readonly HashSet<CharacterModule> _modules = new();
    readonly CharacterEvents          _events  = new();

    public string team = "";
    public Vector3 Position => transform.position;
    public CharacterEvents Events => _events;
    public IReadOnlyCollection<CharacterModule> Modules => _modules;
    public bool AddModule(CharacterModule module) => _modules.Add(module);
    public bool RemoveModule(CharacterModule module) => _modules.Remove(module);
    public T GetModule<T>() where T : CharacterModule => _modules.OfType<T>().FirstOrDefault();
    public bool TryGetModule<T>(out T module) where T : CharacterModule
    {
        module = GetModule<T>();
        return module != null;
    }
    void Start()
    {
        _events.onDied += OnDied;
    }
    void OnDied(ICharacter character) => Pool.Release(this);
    public enum State
    {
        // InControl:
        InControl,
        Idle,
        Chasing,
        Attacking,
        // Yielded:
        Yielded,
        Controlled,
        Dying,
    }
    public GenericPool<MonoBehaviour> Pool { get; set; }
    public void OnGet()
    {
        _events.onInitialized?.Invoke();
    }
    public void OnRelease() {}
    public void OnCreated() {}
}