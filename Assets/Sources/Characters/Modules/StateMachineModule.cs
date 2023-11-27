using System;
using Sources.Systems.FSM;
using UnityEngine;
using static Character.State;

namespace Sources.Characters.Modules
{
    public class StateMachineModule : CharacterModule
    {
        readonly StateMachine<Character.State> _stateMachine = new(Idle);
        public StateMachine<Character.State> StateMachine => _stateMachine;
        protected override void Init()
        {
            _stateMachine.SetSubState(InControl, Idle);
            _stateMachine.SetSubState(InControl, Chasing);
            _stateMachine.SetSubState(InControl, Attacking);
            _stateMachine.SetSubState(Yielded, Controlled);
            _stateMachine.SetSubState(Yielded, Dying);
        }
        void Update() => _stateMachine.Update();
    }
}