using System;

namespace Sources.Systems.FSM
{
    public abstract class BaseState<TEnum> : IState<TEnum> where TEnum : Enum
    {
        public TEnum StateEnum { get; }
        protected BaseState(TEnum stateEnum, StateMachine<TEnum> stateMachine)
        {
            StateEnum = stateEnum;
            this.AddStateListeners(stateMachine);
        }
        public virtual void Enter() {}
        public virtual void FixedUpdate() {}
        public virtual void Update() {}
        public virtual void Exit() {}
    }
}