using System;

namespace Sources.Systems.FSM
{
    public static class StateExtension
    {
        public static void AddTransitionListeners<TContext, TEnum>(
            this IState<TContext, TEnum> state,
            StateMachine<TContext, TEnum> stateMachine)
            where TEnum : Enum
        {
            var stateEnum = state.StateEnum;
            stateMachine[LifeCycle.Enter, stateEnum] += state.Enter;
            stateMachine[LifeCycle.Exit, stateEnum] += state.Exit;
        }
    }
}