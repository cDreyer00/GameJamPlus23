using System;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

public class MeleeEnemySm : StateMachineModule<MeleeEnemySm, Character.State>
{
    HealthModule                 _healthModule;
    NavMeshMovement              _movementModule;
    MeleeAttack                  _attackModule;
    [SerializeField] Animator    animator;
    [SerializeField] MMFeedbacks hitFeedBack;
    [SerializeField] MMFeedbacks explosionFeedBack;
    [SerializeField] float poolCollectDelay = 1f;
    protected override Character.State InitialState => Idle;
    protected override MeleeEnemySm Context => this;

    readonly int  _hWalkBool      = Animator.StringToHash("isWalk");
    readonly int  _hDeadTrigger   = Animator.StringToHash("isDead");
    readonly int  _hAttackTrigger = Animator.StringToHash("isAttack");
    Action<float> _onTakeDamage;

    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        _attackModule = Character.GetModule<MeleeAttack>();
        _movementModule.Target = GameManager.Instance.Player.transform;
        _healthModule = Character.GetModule<HealthModule>();
        _onTakeDamage = OnTakeDamage;
        IdleState();
        ChasingState();
        DyingState();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        _healthModule.OnTakeDamage += _onTakeDamage;
    }
    void OnDisable()
    {
        _healthModule.OnTakeDamage -= _onTakeDamage;
    }
    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }
    void DyingState()
    {
        stateMachine.Transition(Dying, static sm => sm._healthModule.Health <= 0);
        stateMachine.From(Dying).AddListener(LifeCycle.Enter, static sm => {
            if (sm.animator) sm.animator.SetTrigger(sm._hDeadTrigger);
            if (sm.explosionFeedBack) {
                //TODO: fix this mother of all hacks
                var t = sm.transform;
                var scale = t.localScale;
                var position = t.position;
                sm.explosionFeedBack.PlayFeedbacks(position);
                t.localScale = Vector3.zero;
                Helpers.Delay(sm.poolCollectDelay, static valueTuple => {
                    var (sm, scale) = valueTuple;
                    valueTuple.sm.Character.Events.Died(sm.Character);
                    sm.transform.localScale = scale;
                }, (sm, scale));
            }
            else {
                sm.Character.Events.Died(sm.Character);
            }
        });
    }
    void OnTakeDamage(float dmg)
    {
        if (hitFeedBack && dmg > 0) hitFeedBack.PlayFeedbacks(transform.position);
    }
    void IdleState()
    {
        stateMachine.Transition(Idle, Chasing, static sm => sm._movementModule.Target);
    }
    void ChasingState()
    {
        stateMachine.From(Attacking).Transition(Chasing, static sm => !sm._attackModule.isAttacking)
            .AddListener(LifeCycle.Enter, static sm => {
                sm._movementModule.StopMovement();
                sm.animator.SetTrigger(sm._hAttackTrigger);
            });

        stateMachine.From(Chasing)
            .Transition(Idle, static sm => !sm._movementModule.Target)
            .Transition(Attacking, static sm => sm._attackModule.isAttacking)
            .AddListener(LifeCycle.Enter, static sm => {
                sm._movementModule.StartChase();
                sm.animator.SetBool(sm._hWalkBool, true);
            })
            .AddListener(LifeCycle.Exit, static sm => {
                sm._movementModule.StopMovement();
                sm.animator.SetBool(sm._hWalkBool, false);
            });
    }
}