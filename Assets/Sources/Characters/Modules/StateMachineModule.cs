using System;
using Sources.Systems.FSM;
using UnityEngine;
using static Character.State;
using static Character;

public abstract class StateMachineModule<TContext, TEnum> : CharacterModule where TEnum : Enum
{
    protected StateMachine<TContext, TEnum> stateMachine;
    abstract protected TEnum InitialState { get; }
    abstract protected TContext Context { get; }
    protected override void Init()
    {
        stateMachine = new StateMachine<TContext, TEnum>(InitialState, Context);
    }
    virtual protected void OnEnable()
    {
        stateMachine.ChangeState(InitialState);
    }
    virtual protected void Update() => stateMachine.Update();
    virtual protected void FixedUpdate() => stateMachine.FixedUpdate();
}