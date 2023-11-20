using System;

namespace Sources.Systems.FSM
{
    public static class StateExtension
    {
        public static void Register<TEnum>(this IState<TEnum> state, StateMachine<TEnum> stateMachine) where TEnum : Enum
        {
            var stateEnum = state.StateEnum;
            stateMachine[LifeCycle.Enter, stateEnum] += state.Enter;
            stateMachine[LifeCycle.FixedUpdate, stateEnum] += state.FixedUpdate;
            stateMachine[LifeCycle.Update, stateEnum] += state.Update;
            stateMachine[LifeCycle.Exit, stateEnum] += state.Exit;
        }
    }
}