using System;

namespace Sources.Systems.FSM
{
    public interface IState<out TEnum> where TEnum : Enum
    {
        public TEnum StateEnum { get; }
        void Enter();
        void FixedUpdate();
        void Update();
        void Exit();
    }
}