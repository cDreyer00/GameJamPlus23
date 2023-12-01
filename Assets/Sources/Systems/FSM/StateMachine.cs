using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Sources.Systems.FSM
{
    [Serializable]
    public class StateMachine<TContext, TState> where TState : Enum
    {
        public TContext Context { get; set; }
        static int LifeCycleEvents => Cached.EnumValues<LifeCycle>().Length;
        static int StateCount => Cached.EnumValues<TState>().Length;
        static int AsInt32(TState state) => UnsafeUtility.As<TState, int>(ref state);
        static int AsInt32(LifeCycle lifeCycle) => (int)lifeCycle;
        public StateMachine(
            TState initialState,
            TContext context = default,
            [CanBeNull] EqualityComparer<TState> stateEqualityComparer = null)
        {
            Context = context;
            CurrentState = initialState;
            _stateEqualityComparer = stateEqualityComparer ?? EqualityComparer<TState>.Default;
        }
        EqualityComparer<TState> _stateEqualityComparer;
        public TState CurrentState { get; private set; }

        SortedList<TState, List<TransitionData>> _transitionsMap    = new();
        SortedList<TState, TState>               _parentState       = new();
        HashSet<TransitionData>                  _anyTransitionsSet = new();

        Action<TContext>[] _eventTable = new Action<TContext>[LifeCycleEvents * StateCount];

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
        public bool TryGetParent(TState state, out TState parent) => _parentState.TryGetValue(state, out parent);
        public void Transition(TState src, TState dst, Func<TContext, bool> predicate = null)
        {
            if (!_transitionsMap.TryGetValue(src, out var transitions)) {
                transitions = new List<TransitionData>();
                _transitionsMap.Add(src, transitions);
            }
            transitions.Add(new TransitionData { state = dst, predicate = predicate ?? (static _ => true) });
        }
        public void Transition(TState dst, Func<TContext, bool> predicate = null)
        {
            _anyTransitionsSet.Add(new TransitionData { state = dst, predicate = predicate ?? (static _ => true) });
        }
        public void RemoveTransition(TState src, TState dst)
        {
            if (!_transitionsMap.TryGetValue(src, out var transitions)) return;
            foreach (var transitionData in transitions) {
                if (_stateEqualityComparer.Equals(transitionData.state, dst)) {
                    transitions.Remove(transitionData);
                    break;
                }
            }
        }
        public void RemoveTransition(TState dst)
        {
            foreach (var transitionData in _anyTransitionsSet) {
                if (_stateEqualityComparer.Equals(transitionData.state, dst)) {
                    _anyTransitionsSet.Remove(transitionData);
                    break;
                }
            }
        }
        public Action<TContext> this[LifeCycle lifeCycle, TState state]
        {
            get => _eventTable[AsInt32(lifeCycle) * LifeCycleEvents + AsInt32(state)];
            set => _eventTable[AsInt32(lifeCycle) * LifeCycleEvents + AsInt32(state)] = value;
        }
        public void ChangeState(TState newState)
        {
            if (_stateEqualityComparer.Equals(CurrentState, newState)) return;

            var oldState = CurrentState;
            CurrentState = newState;

            _eventTable[AsInt32(LifeCycle.Exit) * LifeCycleEvents + AsInt32(oldState)]?.Invoke(Context);
            _eventTable[AsInt32(LifeCycle.Enter) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke(Context);

            foreach (var parent in GetParents(oldState)) {
                _eventTable[AsInt32(LifeCycle.Exit) * LifeCycleEvents + AsInt32(parent)]?.Invoke(Context);
            }
            foreach (var parent in GetParents(CurrentState)) {
                _eventTable[AsInt32(LifeCycle.Enter) * LifeCycleEvents + AsInt32(parent)]?.Invoke(Context);
            }
        }

        TransitionData? GetTransition()
        {
            foreach (var transition in _anyTransitionsSet) {
                if (transition.predicate(Context)) return transition;
            }
            if (_transitionsMap.TryGetValue(CurrentState, out var transitionState)) {
                foreach (var transition in transitionState) {
                    if (transition.predicate(Context)) return transition;
                }
            }
            return default;
        }
        public void Update()
        {
            var transition = GetTransition();
            if (transition != null) ChangeState(transition.Value.state);

            _eventTable[AsInt32(LifeCycle.Update) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke(Context);
        }
        public void FixedUpdate()
        {
            _eventTable[AsInt32(LifeCycle.FixedUpdate) * LifeCycleEvents + AsInt32(CurrentState)]?.Invoke(Context);
        }
        public struct TransitionData
        {
            public TState               state;
            public Func<TContext, bool> predicate;
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