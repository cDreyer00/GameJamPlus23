using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour, ICharacter
{
    StateMachine<State>      _stateMachine = new(State.Idle);
    readonly HashSet<CharacterModule> _modules = new();
    public Vector3 Position => transform.position;
    readonly CharacterEvents          _events  = new();
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

    void Awake() {
            
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