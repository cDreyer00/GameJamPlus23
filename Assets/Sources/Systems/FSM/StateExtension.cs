using System;

namespace Sources.Systems.FSM
{
    public static class StateExtension
    {
        public static void AddTransitionListeners<TEnum>(this IState<TEnum> state, StateMachine<TEnum> stateMachine) where TEnum : Enum
        {
            var stateEnum = state.StateEnum;
            stateMachine[LifeCycle.Enter, stateEnum] += state.Enter;
            stateMachine[LifeCycle.Exit, stateEnum] += state.Exit;
        }
    }
}