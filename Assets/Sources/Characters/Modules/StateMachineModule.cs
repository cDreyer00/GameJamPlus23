using System;
using Sources.Systems.FSM;

namespace Sources.Characters.Modules
{
    public class StateMachineModule : CharacterModule
    {
        readonly StateMachine<Character.State> _stateMachine = new(Character.State.Idle);
        public StateMachine<Character.State> StateMachine => _stateMachine;
        protected override void Init()
        {
            _stateMachine.SetSubState(Character.State.InControl, Character.State.Idle);
            _stateMachine.SetSubState(Character.State.InControl, Character.State.Chasing);
            _stateMachine.SetSubState(Character.State.InControl, Character.State.Attacking);
            _stateMachine.SetSubState(Character.State.Yielded, Character.State.Controlled);
            _stateMachine.SetSubState(Character.State.Yielded, Character.State.Dying);
        }
        void Update() => _stateMachine.Update();
    }
}