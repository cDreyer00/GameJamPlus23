using System;

namespace Sources.Systems.FSM
{
    public interface IState<in TContext,out TEnum> where TEnum : Enum
    {
        public TEnum StateEnum { get; }
        void Enter(TContext context);
        void FixedUpdate(TContext context);
        void Update(TContext context);
        void Exit(TContext context);
    }
}