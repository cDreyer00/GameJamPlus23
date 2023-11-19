using System;
using UnityEngine;
using UnityEngine.AI;
using Sources.Systems.FSM;
using static Character;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform    target;
    [SerializeField] float        startCooldownTime;
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
        var fsm = Character.StateMachine;
        fsm.Transition(State.Idle, () => target == null);
        fsm.Transition(State.Idle, State.Chasing, () => target != null);
        fsm[LifeCycle.Enter, State.Chasing] += () => agent.isStopped = false;
        fsm[LifeCycle.Exit, State.Chasing] += () => agent.isStopped = true;
        fsm[LifeCycle.Update, State.Chasing] += () => SetDestination(target.position);
        Helpers.Delay(startCooldownTime, static self => self.SetTarget(GameManager.Instance.Player.transform), this);
    }
    protected override void Init()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }
    public void SetDestination(Vector3 position) => agent.SetDestination(position);
}