using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StateVariable
{
    Idle,
    Chasing,
    Attacking,
    Controlled,
}
public enum EventVariable
{
    Stop,
    Chase,
    Attack,
    YieldControl,
}
[Serializable]
public struct Transition
{
    public StateVariable origin;
    public EventVariable eventVariable;
    public StateVariable destination;
}

[Serializable]
public class StateMachine : IEnumerable<Transition>
{
    public List<Transition> transitions = new();
    public StateVariable    currentState;

    StateVariable this[StateVariable origin, EventVariable eventVariable]
    {
        get
        {
            int index = transitions.FindIndex(Exists);
            return index == -1 ? origin : transitions[index].destination;
            bool Exists(Transition t) => t.origin == origin && t.eventVariable == eventVariable;
        }
        set => Allow(origin, eventVariable, value);
    }

    public static StateMachine FullyConnectedGraph => new()
    {
        currentState = StateVariable.Idle,
        // Idle
        [StateVariable.Idle, EventVariable.Chase] = StateVariable.Chasing,
        [StateVariable.Idle, EventVariable.Attack] = StateVariable.Attacking,
        [StateVariable.Idle, EventVariable.YieldControl] = StateVariable.Controlled,
        // Chasing
        [StateVariable.Chasing, EventVariable.Stop] = StateVariable.Idle,
        [StateVariable.Chasing, EventVariable.Attack] = StateVariable.Attacking,
        [StateVariable.Chasing, EventVariable.YieldControl] = StateVariable.Controlled,
        // Attacking
        [StateVariable.Attacking, EventVariable.Stop] = StateVariable.Idle,
        [StateVariable.Attacking, EventVariable.Chase] = StateVariable.Chasing,
        [StateVariable.Attacking, EventVariable.YieldControl] = StateVariable.Controlled,
        // Controlled
        [StateVariable.Controlled, EventVariable.Stop] = StateVariable.Idle,
        [StateVariable.Controlled, EventVariable.Chase] = StateVariable.Chasing,
        [StateVariable.Controlled, EventVariable.Attack] = StateVariable.Attacking,
    };

    public void Allow(StateVariable origin, EventVariable eventVariable, StateVariable destination)
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

    public void Forbid(StateVariable origin, EventVariable eventVariable, StateVariable destination)
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

    public event Action<StateVariable> onStateEnter;
    public event Action<StateVariable> onStateExit;

    virtual protected void StateEnter(StateVariable obj)
    {
        onStateEnter?.Invoke(obj);
    }
    virtual protected void StateExit(StateVariable obj)
    {
        onStateExit?.Invoke(obj);
    }

    public void MoveNext(EventVariable eventVariable)
    {
        int index = transitions.FindIndex(Exists);
        if (index != -1) {
            StateExit(currentState);
            currentState = transitions[index].destination;
            StateEnter(currentState);
        }
        bool Exists(Transition t) => t.origin == currentState && t.eventVariable == eventVariable;
    }
    public IEnumerator<Transition> GetEnumerator() => transitions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}