using System;
using System.Collections.Generic;
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
        EqualityComparer<TState> _stateEqualityComparer = EqualityComparer<TState>.Default;
        public TState CurrentState { get; private set; }

        Dictionary<TState, List<Destination>> _transitionsMap    = new();
        HashSet<Destination>                  _anyTransitionsSet = new();

        Dictionary<TState, Action> _onEnter       = new();
        Dictionary<TState, Action> _onExit        = new();
        Dictionary<TState, Action> _onUpdate      = new();
        Dictionary<TState, Action> _onFixedUpdate = new();

        public void AddTransition(TState from, TState to, Func<bool> predicate = null)
        {
            if (!_transitionsMap.TryGetValue(from, out var transitions)) {
                transitions = new List<Destination>();
                _transitionsMap.Add(from, transitions);
            }
            transitions.Add(new Destination { state = to, predicate = predicate ?? (() => true) });
        }
        public void AddAnyTransition(TState to, Func<bool> predicate = null)
        {
            _anyTransitionsSet.Add(new Destination { state = to, predicate = predicate ?? (() => true) });
        }
        public void AddListener(TState state, LifeCycle lifeCycleEvent, Action action)
        {
            switch (lifeCycleEvent) {
            case LifeCycle.Enter:
                _onEnter[state] += action;
                break;
            case LifeCycle.Exit:
                _onExit[state] += action;
                break;
            case LifeCycle.Update:
                _onUpdate[state] += action;
                break;
            case LifeCycle.FixedUpdate:
                _onFixedUpdate[state] += action;
                break;
            }
        }
        public void RemoveListener(TState state, LifeCycle lifeCycleEvent, Action action)
        {
            switch (lifeCycleEvent) {
            case LifeCycle.Enter:
                _onEnter[state] -= action;
                break;
            case LifeCycle.Exit:
                _onExit[state] -= action;
                break;
            case LifeCycle.Update:
                _onUpdate[state] -= action;
                break;
            case LifeCycle.FixedUpdate:
                _onFixedUpdate[state] -= action;
                break;
            }
        }
        public void ChangeState(TState newState)
        {
            if (_stateEqualityComparer.Equals(CurrentState, newState)) return;

            var oldState = CurrentState;
            CurrentState = newState;

            _onExit[oldState]?.Invoke();
            _onEnter[CurrentState]?.Invoke();
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

            _onUpdate[CurrentState]?.Invoke();
        }

        public void FixedUpdate()
        {
            _onFixedUpdate[CurrentState]?.Invoke();
        }
        public struct Destination
        {
            public TState     state;
            public Func<bool> predicate;
        }
    }
}