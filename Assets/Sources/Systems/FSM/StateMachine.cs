﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Sources.Systems.FSM
{
    [Serializable]
    public class StateMachine<TState> where TState : Enum
    {
        static int LifeCycleEvents => Cached.EnumValues<LifeCycle>().Length;
        static int StateCount => Cached.EnumValues<TState>().Length;
        static int AsInt32(TState state) => UnsafeUtility.As<TState, int>(ref state);
        static int AsInt32(LifeCycle lifeCycle) => (int)lifeCycle;
        public StateMachine(TState initialState, [CanBeNull] EqualityComparer<TState> stateEqualityComparer = null)
        {
            CurrentState = initialState;
            _stateEqualityComparer = stateEqualityComparer ?? EqualityComparer<TState>.Default;
        }
        EqualityComparer<TState> _stateEqualityComparer;
        public TState CurrentState { get; private set; }

        SortedList<TState, List<Destination>> _transitionsMap    = new();
        SortedList<TState, TState>            _parentState       = new();
        HashSet<Destination>                  _anyTransitionsSet = new();

        Action[] _eventTable = new Action[LifeCycleEvents * StateCount];

        public void SetSubState(TState parent, TState child)
        {
            _parentState[child] = parent;
        }
        public IEnumerable<TState> GetParents(TState state)
        {
            while (_parentState.TryGetValue(state, out var parent)) {
                yield return parent;
                state = parent;
            }
        }
        public bool TryGetParent(TState state, out TState parent)
        {
            return _parentState.TryGetValue(state, out parent);
        }
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

            foreach (var parent in GetParents(oldState)) {
                _eventTable[AsInt32(LifeCycle.Exit) * LifeCycleEvents + AsInt32(parent)]?.Invoke();
            }
            foreach (var parent in GetParents(CurrentState)) {
                _eventTable[AsInt32(LifeCycle.Enter) * LifeCycleEvents + AsInt32(parent)]?.Invoke();
            }
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
    public enum LifeCycle
    {
        Enter,
        FixedUpdate,
        Update,
        Exit
    }
}