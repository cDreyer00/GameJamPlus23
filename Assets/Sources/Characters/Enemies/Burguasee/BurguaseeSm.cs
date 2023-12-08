using System;
using DG.Tweening;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static BorguaseState;

public class BurguaseeSm : StateMachineModule<BurguaseeSm, BorguaseState>
{
    [SerializeField] float    slamRange;
    [SerializeField] Vector2  dashCooldown;
    [SerializeField] Animator animator;


    readonly int _hAttackTrigger = Animator.StringToHash("attack");
    readonly int _walkBool       = Animator.StringToHash("isWalk");

    public AttackEventEmitter hammerAttackEvent;
    
    Transform                 _target;
    NavMeshMovement           _movementModule;
    float                     _dashCooldownTimer;
    float                     _slamCooldownTimer;
    protected override BorguaseState InitialState => Idle;
    protected override BurguaseeSm Context => this;

    [SerializeField] BorguaseState state;

    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        hammerAttackEvent.AddListener(() => animator.SetTrigger(_hAttackTrigger));
        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;

        IdleState();
        HammerSlamState();
        ChasingState();
    }
    protected override void Update()
    {
        state = stateMachine.CurrentState;
        base.Update();
        _dashCooldownTimer = Math.Max(_dashCooldownTimer - Time.deltaTime, 0);
        _slamCooldownTimer = Math.Max(_slamCooldownTimer - Time.deltaTime, 0);
    }

    void IdleState()
    {
        stateMachine.From(Idle)
            .Transition(Chasing, sm => sm._movementModule.Target)
            .Transition(HammerSlam, HammerSlamRangePredicate);
    }
    void HammerSlamState()
    {
        stateMachine.From(HammerSlam)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Chasing, sm => !HammerSlamRangePredicate(sm))
            .SetCallback(LifeCycle.Enter, sm => {
                sm.animator.SetTrigger(sm._hAttackTrigger);
                sm.hammerAttackEvent.StartAttack();
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.hammerAttackEvent.StopAttack();
            }).SetCallback(LifeCycle.Update, sm => {
                var smTransform = sm.transform;
                var targetPos   = sm._target.position;
                var position    = smTransform.position;
                var direction   = Vector3Ext.Direction(position, targetPos);
                var rotation    = Quaternion.LookRotation(direction);
                rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                smTransform.rotation = rotation;
            });
    }
    void ChasingState()
    {
        stateMachine.From(Chasing)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .SetCallback(LifeCycle.Enter, sm => {
                sm.animator.SetBool(sm._walkBool, true);
                sm._movementModule.StartChase();
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.animator.SetBool(sm._walkBool, false);
                sm._movementModule.StopMovement();
            });
    }
    static bool HammerSlamRangePredicate(BurguaseeSm sm)
    {
        var   targetPos = sm._target.position;
        var   position  = sm.transform.position;
        bool  canSlam   = sm._slamCooldownTimer <= 0;
        float distSqr   = Vector3Ext.SqrDistance(position, targetPos);
        float rangeTip  = sm.slamRange;
        return canSlam & distSqr < Mathf.Pow(rangeTip, 2);
    }
    void OnCollisionEnter(Collision collision)
    {
        bool hitPlayer = collision.gameObject.CompareTag("Player");

        if (stateMachine.CurrentState == Dash) {
            if (hitPlayer) {
                stateMachine.ChangeState(HammerSlam);
            }
            stateMachine.ChangeState(Chasing);
        }
    }
}
public enum BorguaseState
{
    Idle,
    Dash,
    HammerSlam,
    Chasing,
}