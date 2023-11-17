using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Sources.Systems.FSM
{
    public enum LifeCycle
    {
        Enter,
        FixedUpdate,
        Update,
        Exit
    }

    [Serializable]
    public class StateMachine<TState>
        where TState : Enum
    {
        static int LifeCycleEvents => Cached.EnumValues<LifeCycle>().Length;
        static int StateCount => Cached.EnumValues<TState>().Length;
        public StateMachine(TState initialState)
        {
            CurrentState = initialState;
        }
        EqualityComparer<TState> _stateEqualityComparer = EqualityComparer<TState>.Default;
        static int AsInt32(TState state) => UnsafeUtility.As<TState, int>(ref state);
        static int AsInt32(LifeCycle lifeCycle) => (int)lifeCycle;

        public TState CurrentState { get; private set; }

        Dictionary<TState, List<Destination>> _transitionsMap    = new();
        HashSet<Destination>                  _anyTransitionsSet = new();

        Action[] _eventTable = new Action[LifeCycleEvents * StateCount];

        public void Transition(TState src, TState dst, Func<bool> predicate = null)
        {
            if (!_transitionsMap.TryGetValue(src, out var transitions)) {
                transitions = new List<Destination>();
                _transitionsMap.Add(src, transitions);
            }
            transitions.Add(new Destination { state = dst, predicate = predicate ?? (() => true) });
        }
        public void Transition(TState dst, Func<bool> predicate = null)
        {
            _anyTransitionsSet.Add(new Destination { state = dst, predicate = predicate ?? (() => true) });
        }
        public Action this[LifeCycle lifeCycle, TState state]
        {
            get => _eventTable[AsInt32(lifeCycle) * LifeCycleEvents + AsInt32(state)];
            set => _eventTable[AsInt32(lifeCycle) * LifeCycleEvents + AsInt32(state)] = value;
        }
        public void ChangeState(TState newState)
        {
            if (_stateEqualityComparer.Equals(CurrentState, newState)) return;

            var oldState = CurrentState;
            CurrentState = newState;

            _eventTable[AsInt32(LifeCycle.Exit) * LifeCycleEvents + AsInt32(oldState)]?.Invoke();
            _eventTable[AsInt32(LifeCycle.Enter) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke();
        }

        Destination? GetTransition()
        {
            foreach (var transition in _anyTransitionsSet) {
                if (transition.predicate()) return transition;
            }
            if (_transitionsMap.TryGetValue(CurrentState, out var transitionState)) {
                foreach (var transition in transitionState) {
                    if (transition.predicate()) return transition;
                }
            }
            return default;
        }

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null) ChangeState(transition.Value.state);

            _eventTable[AsInt32(LifeCycle.Update) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke();
        }

        public void FixedUpdate()
        {
            _eventTable[AsInt32(LifeCycle.FixedUpdate) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke();
        }
        public struct Destination
        {
            public TState     state;
            public Func<bool> predicate;
        }
    }
}