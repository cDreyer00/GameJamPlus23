using System;
using DG.Tweening;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static BurgerBotState;

public class BurgerBotSm : StateMachineModule<BurgerBotSm, BurgerBotState>
{
    [SerializeField] float    slamRange;
    [SerializeField] float    slamCooldown;
    [SerializeField] Vector2  dashCooldown;
    [SerializeField] Animator animator;

    readonly int _hAttackTrigger    = Animator.StringToHash("isAttack");
    readonly int _hMarteladaTrigger = Animator.StringToHash("martelada");
    readonly int _hWalkBool         = Animator.StringToHash("isWalk");
    readonly int _hIdleBool         = Animator.StringToHash("isIdle");
    readonly int _hDashTrigger      = Animator.StringToHash("dash");

    public AttackEventEmitter hammerAttackEvent;
    public AttackEventEmitter dashAttackEvent;
    NavMeshMovement           _movementModule;
    Transform                 _target;
    float                     _dashCooldownTimer;
    float                     _slamCooldownTimer;
    protected override BurgerBotState InitialState => Idle;
    protected override BurgerBotSm Context => this;

    [SerializeField] BurgerBotState state;

    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        hammerAttackEvent.AddListener(() => {
            animator.SetTrigger(_hAttackTrigger);
            animator.SetTrigger(_hMarteladaTrigger);
        });
        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;

        IdleState();
        HammerSlamState();
        DashState();
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
            .Transition(Dash, DashPredicate)
            .Transition(Chasing, sm => sm._movementModule.Target)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .SetCallback(LifeCycle.Enter, sm => {
                sm.animator.SetBool(sm._hIdleBool, true);
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.animator.SetBool(sm._hIdleBool, false);
            });
    }
    void DashState()
    {
        stateMachine.From(Dash)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .SetCallback(LifeCycle.Enter, sm => {
                TurnToTarget(sm);
                sm.dashAttackEvent.StartAttack();
                sm.animator.SetTrigger(sm._hAttackTrigger);
                sm.animator.SetTrigger(sm._hDashTrigger);
                sm.Delay(Time.deltaTime * 33.34f, sm => sm._movementModule.StartDash());
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.dashAttackEvent.StopAttack();
                sm._movementModule.DashTween.Kill();
                sm._dashCooldownTimer = UnityEngine.Random.Range(sm.dashCooldown.x, sm.dashCooldown.y);
            });
    }
    void HammerSlamState()
    {
        stateMachine.From(HammerSlam)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Chasing, sm => !HammerSlamRangePredicate(sm))
            .SetCallback(LifeCycle.Enter, sm => {
                sm.animator.SetTrigger(sm._hAttackTrigger);
                sm.animator.SetTrigger(sm._hMarteladaTrigger);
                sm.hammerAttackEvent.StartAttack();
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.hammerAttackEvent.StopAttack();
                sm._slamCooldownTimer = sm.slamCooldown;
            })
            .SetCallback(LifeCycle.Update, TurnToTarget);
    }
    static void TurnToTarget(BurgerBotSm sm)
    {
        var smTransform = sm.transform;
        var targetPos   = sm._target.position;
        var position    = smTransform.position;
        var direction   = Vector3Ext.Direction(position, targetPos);
        var rotation    = Quaternion.LookRotation(direction);
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        smTransform.rotation = rotation;
    }
    void ChasingState()
    {
        stateMachine.From(Chasing)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Dash, DashPredicate)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .SetCallback(LifeCycle.Enter, sm => {
                sm.animator.SetBool(sm._hWalkBool, true);
                sm._movementModule.StartChase();
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm.animator.SetBool(sm._hWalkBool, false);
                sm._movementModule.StopMovement();
            });
    }
    static bool HammerSlamRangePredicate(BurgerBotSm sm)
    {
        var   targetPos = sm._target.position;
        var   position  = sm.transform.position;
        bool  canSlam   = sm._slamCooldownTimer <= 0;
        float distSqr   = Vector3Ext.SqrDistance(position, targetPos);
        float rangeTip  = sm.slamRange;
        return canSlam & distSqr < Mathf.Pow(rangeTip, 2);
    }
    static bool DashPredicate(BurgerBotSm sm)
    {
        bool canDash    = sm._dashCooldownTimer <= 0 && sm.dashAttackEvent;
        bool outOfRange = !HammerSlamRangePredicate(sm);
        bool hasTarget  = sm._movementModule.Target;
        return canDash & hasTarget & outOfRange;
    }
    void OnCollisionEnter(Collision collision)
    {
        bool witWall   = collision.gameObject.CompareTag("Wall");
        bool witPlayer = collision.gameObject.CompareTag("Player");

        if (stateMachine.CurrentState == Dash & (witWall | witPlayer)) {
            stateMachine.ChangeState(Chasing);
        }
    }
}
public enum BurgerBotState
{
    Idle,
    Dash,
    HammerSlam,
    Chasing,
}