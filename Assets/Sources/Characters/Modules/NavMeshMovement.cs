using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField] NavMeshAgent _agent;
    //[SerializeField] MoveType     _moveType;
    [SerializeField] Transform    _target;

    public NavMeshAgent Agent => _agent;
    public Transform Target
    {
        get => _target;
        set => _target = value;
    }
    // public MoveType MoveType
    // {
    //     get => _moveType;
    //     set => _moveType = value;
    // }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (!_agent) return;

        // if (_moveType == MoveType.Chase) {
        //     if (!_target) return;
        //     SetDestination(_target.position);
        // }
        // if (_moveType == MoveType.Idle) {
        //     SetDestination(transform.position);
        // }

        if (Character.stateMachine.currentState == FsmCharState.Chasing) {
            if (!_target) return;
            SetDestination(_target.position);
        }
        if (Character.stateMachine.currentState == FsmCharState.Idle) {
            SetDestination(transform.position);
        }
    }

    protected override void Init()
    {
        if (_agent == null)
            _agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 position)
    {
        _agent.SetDestination(position);
    }
}

public enum MoveType
{
    Chase,
    Idle,
    Random
}