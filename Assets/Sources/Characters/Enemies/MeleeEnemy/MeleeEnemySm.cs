using System;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

public class MeleeEnemySm : StateMachineModule<MeleeEnemySm, Character.State>
{
    NavMeshMovement _movementModule;
    MeleeAttack     _attackModule;

    [SerializeField] Animator animator;
    protected override Character.State InitialState => Idle;
    protected override MeleeEnemySm Context => this;

    readonly int _hWalkBool      = Animator.StringToHash("isWalk");
    readonly int _hDeadTrigger   = Animator.StringToHash("isDead");
    readonly int _hAttackTrigger = Animator.StringToHash("isAttack");
    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        _attackModule = Character.GetModule<MeleeAttack>();
        _movementModule.Target = GameManager.Instance.Player.transform;
        IdleState();
        ChasingState();
    }
    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }
    void IdleState()
    {
        stateMachine.Transition(Idle, Chasing, static sm => sm._movementModule.Target);
    }
    void ChasingState()
    {
        stateMachine.Transition(Chasing, Idle, static sm => !sm._movementModule.Target);
        stateMachine.Transition(Attacking, static sm => sm._attackModule.isAttacking);
        stateMachine.Transition(Attacking, Chasing, static sm => !sm._attackModule.isAttacking);

        stateMachine[LifeCycle.Enter, Attacking] = static sm => {
            sm._movementModule.StopMovement();
            sm.animator.SetTrigger(sm._hAttackTrigger);
        };
        stateMachine[LifeCycle.Enter, Chasing] = static sm => {
            sm._movementModule.StartChase();
            sm.animator.SetBool(sm._hWalkBool, true);
        };
        stateMachine[LifeCycle.Exit, Chasing] = static sm => {
            sm._movementModule.StopMovement();
            sm.animator.SetBool(sm._hWalkBool, false);
        };
    }
}