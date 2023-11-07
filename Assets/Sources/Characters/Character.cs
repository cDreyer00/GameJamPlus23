using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Character : MonoBehaviour, ICharacter
{
    public StateMachine<FsmCharState, FsmCharEvent> stateMachine;

    int                               _referenceCount;
    readonly HashSet<CharacterModule> _modules = new();
    readonly CharacterEvents          _events  = new();


    public abstract string Team { get; }
    public int ReferenceCount
    {
        get => Mathf.Max(0, _referenceCount);
        set => _referenceCount = Mathf.Max(0, value);
    }
    public Vector3 Position => transform.position;
    public CharacterEvents Events => _events;
    public IReadOnlyCollection<CharacterModule> Modules => _modules;

    public void AddModule(CharacterModule module)
    {
        _modules.Add(module);
    }

    public void RemoveModule(CharacterModule module)
    {
        _modules.Remove(module);
    }

    public CharacterModule GetModule<T>() where T : CharacterModule
    {
        return _modules.FirstOrDefault(m => m is T);
    }
}
public enum FsmCharState
{
    Idle,
    Chasing,
    Attacking,
    Controlled,
    Dying,
    Finalized,
}
public enum FsmCharEvent
{
    Stop,
    Chase,
    Attack,
    YieldControl,
    Die,
    Finalize,
}