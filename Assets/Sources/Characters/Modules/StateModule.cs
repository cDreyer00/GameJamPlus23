using System;
using JetBrains.Annotations;
using Sources.Systems.FSM;
using UnityEngine.Serialization;

namespace Sources.Characters.Modules
{
    public abstract class StateModule : CharacterModule, IState<Character.State>
    {
        StateMachine<Character.State> _stateMachine;
        public abstract Character.State StateEnum { get; }
        public abstract void Enter();
        public abstract void FixedUpdate();
        public abstract void Update();
        public abstract void Exit();
        protected override void Init()
        {
            if (Character.TryGetModule<StateMachineModule>(out var stateMachineModule)) {
                _stateMachine = stateMachineModule.StateMachine;
                this.AddStateListeners(_stateMachine);
            }
        }
        public void FromState(Character.State src, Func<bool> predicate = null) =>
            _stateMachine.Transition(src, StateEnum, predicate);
        public void ToState(Character.State dst, Func<bool> predicate = null) =>
            _stateMachine.Transition(StateEnum, dst, predicate);
        public void FromAny(Func<bool> predicate = null) =>
            _stateMachine.Transition(StateEnum, predicate);
    }
}