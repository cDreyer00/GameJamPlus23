using System;

namespace Sources.Systems.FSM
{
    public readonly struct StateConfigurator<TContext, TEnum> where TEnum : Enum
    {
        readonly StateMachine<TContext, TEnum> _stateMachine;
        readonly TEnum                         _state;
        public StateConfigurator(TEnum state, StateMachine<TContext, TEnum> stateMachine)
        {
            this._stateMachine = stateMachine;
            _state = state;
        }
        public StateConfigurator<TContext, TEnum> Transition(TEnum dst, Func<TContext, bool> predicate = null)
        {
            _stateMachine.Transition(_state, dst, predicate);
            return this;
        }
        public StateConfigurator<TContext, TEnum> AddListener(LifeCycle lifeCycle, Action<TContext> action)
        {
            _stateMachine[lifeCycle, _state] += action;
            return this;
        }
        public StateConfigurator<TContext, TEnum> RemoveListener(LifeCycle lifeCycle, Action<TContext> action)
        {
            _stateMachine[lifeCycle, _state] -= action;
            return this;
        }
        public StateConfigurator<TContext, TEnum> SetCallback(LifeCycle lifeCycle, Action<TContext> action)
        {
            _stateMachine[lifeCycle, _state] = action;
            return this;
        }
        public StateConfigurator<TContext, TEnum> SetSubState(TEnum parent, TEnum child)
        {
            _stateMachine.SetSubState(parent, child);
            return this;
        }
    }
}