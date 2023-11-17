using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Character : MonoBehaviour, ICharacter
{
    readonly StateMachine<State>      _stateMachine = new(State.Idle);
    readonly HashSet<CharacterModule> _modules = new();
    readonly CharacterEvents          _events  = new();
    public abstract string Team { get; }
    public Vector3 Position => transform.position;
    public CharacterEvents Events => _events;
    public StateMachine<State> StateMachine => _stateMachine;
    public IReadOnlyCollection<CharacterModule> Modules => _modules;
    public bool AddModule(CharacterModule module) => _modules.Add(module);
    public bool RemoveModule(CharacterModule module) => _modules.Remove(module);
    public T GetModule<T>() where T : CharacterModule => _modules.OfType<T>().FirstOrDefault();
    public bool TryGetModule<T>(out T module) where T : CharacterModule
    {
        module = GetModule<T>();
        return module != null;
    }
    void Update()
    {
        _stateMachine.Update();
    }
    public enum State
    {
        Idle,
        Chasing,
        Attacking,
        Controlled,
        Dying,
        Finalized,
    }
}