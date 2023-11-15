using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class StateMachine<TState, TEvent> : IEnumerable<StateMachine<TState, TEvent>.Transition>
    where TState : Enum
    where TEvent : Enum
{
    EqualityComparer<TState> _stateEq = EqualityComparer<TState>.Default;
    EqualityComparer<TEvent> _eventEq = EqualityComparer<TEvent>.Default;

    public List<Transition> transitions = new();
    public TState           currentState;

    public TState this[TState state, TEvent @event]
    {
        get => GetState(state, @event);
        set => Add(state, @event, value);
    }

    public void Add(TState origin, TEvent eventVariable, TState destination)
    {
        var transition = new Transition
        {
            origin = origin,
            eventVariable = eventVariable,
            destination = destination,
        };
        if (!transitions.Contains(transition)) {
            transitions.Add(transition);
        }
    }
    public void Remove(TState origin, TEvent eventVariable, TState destination)
    {
        var transition = new Transition
        {
            origin = origin,
            eventVariable = eventVariable,
            destination = destination,
        };
        if (transitions.Contains(transition)) {
            transitions.Remove(transition);
        }
    }
    public event Action<TState> OnStateEnter;
    public event Action<TState> OnStateExit;

    virtual protected void StateEnter(TState obj)
    {
        OnStateEnter?.Invoke(obj);
    }
    virtual protected void StateExit(TState obj)
    {
        OnStateExit?.Invoke(obj);
    }
    public TState PeekNext(TEvent eventVariable)
    {
        int index = transitions.FindIndex(TransitionPredicate);
        return index != -1 ? transitions[index].destination : currentState;

        bool TransitionPredicate(Transition t)
        {
            return _stateEq.Equals(t.origin, currentState) && _eventEq.Equals(t.eventVariable, eventVariable);
        }
    }
    public TState GetState(TState state, TEvent eventVariable)
    {
        int index = transitions.FindIndex(TransitionPredicate);
        return index != -1 ? transitions[index].destination : state;

        bool TransitionPredicate(Transition t)
        {
            return _stateEq.Equals(t.origin, state) && _eventEq.Equals(t.eventVariable, eventVariable);
        }
    }
    public void MoveNext(TEvent eventVariable)
    {
        int index = transitions.FindIndex(TransitionPredicate);
        if (index != -1) {
            StateExit(currentState);
            currentState = transitions[index].destination;
            StateEnter(currentState);
        }

        bool TransitionPredicate(Transition t)
        {
            return _stateEq.Equals(t.origin, currentState) && _eventEq.Equals(t.eventVariable, eventVariable);
        }
    }
    public IEnumerator<Transition> GetEnumerator() => transitions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Serializable]
    public struct Transition
    {
        public TState origin;
        public TEvent eventVariable;
        public TState destination;
    }
}