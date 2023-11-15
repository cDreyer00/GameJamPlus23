using System;
using System.Collections;
using System.Collections.Generic;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using static Character;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Transform target;
    [SerializeField]
    StateMachine<State> fsm;
    public NavMeshAgent Agent => agent;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    void Start()
    {
        fsm ??= Character.stateMachine;
        fsm.AddAnyTransition(State.Idle, () => target == null);
        fsm.AddTransition(State.Idle, State.Chasing, () => target != null);
        fsm.AddListener(State.Chasing, LifeCycle.Enter, () => agent.isStopped = false);
        fsm.AddListener(State.Chasing, LifeCycle.Exit, () => agent.isStopped = true);
        fsm.AddListener(State.Chasing, LifeCycle.Update, () => SetDestination(target.position));
    }
    protected override void Init()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }
    public void SetDestination(Vector3 position) => agent.SetDestination(position);
}

public enum MoveType
{
    Chase,
    Idle,
    Random
}