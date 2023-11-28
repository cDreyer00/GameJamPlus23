using System.Collections;
using System.Collections.Generic;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

public class HammerBotSm : StateMachineModule
{
    [SerializeField] float attackRange;

    NavMeshMovement _movementModule;
    HammerAttack    _hammerAttack;
    float SqrDistanceToTarget => (_movementModule.Target.position - transform.position).sqrMagnitude;
    protected override void Init()
    {
        base.Init();

        _movementModule = Character.GetModule<NavMeshMovement>();
        _hammerAttack = Character.GetModule<HammerAttack>();

        var target = GameManager.Instance.Player.transform;
        _movementModule.Target = target;
        _hammerAttack.Target = target;
        _hammerAttack.AttackRange = attackRange;

        StateMachine.Transition(Idle, Chasing, () => _movementModule.Target);
        StateMachine.Transition(Chasing, Idle, () => !_movementModule.Target);
        StateMachine.Transition(Chasing, Attacking, () => SqrDistanceToTarget <= attackRange * attackRange);
        StateMachine.Transition(Attacking, Chasing, () => SqrDistanceToTarget > attackRange * attackRange);

        StateMachine[LifeCycle.Enter, Chasing] += () => _movementModule.StartModule();
        StateMachine[LifeCycle.Exit, Chasing] += () => _movementModule.StopModule();
        StateMachine[LifeCycle.Enter, Attacking] += () => _hammerAttack.StartModule();
        StateMachine[LifeCycle.Exit, Attacking] += () => _hammerAttack.StopModule();
    }
}