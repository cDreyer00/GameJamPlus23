using System;
using DG.Tweening;
using MoreMountains.Feedbacks;
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
    [SerializeField] float    poolCollectDelay = 1f;
    [SerializeField] MMFeedbacks explosionFeedback;
    [SerializeField] MMFeedbacks hitFeedback;
    [SerializeField] MMFeedbacks lowLifeFeedback;

    readonly int _hAttackTrigger    = Animator.StringToHash("isAttack");
    readonly int _hMarteladaTrigger = Animator.StringToHash("martelada");
    readonly int _hWalkBool         = Animator.StringToHash("isWalk");
    readonly int _hIdleBool         = Animator.StringToHash("isIdle");
    readonly int _hDashTrigger      = Animator.StringToHash("dash");

    public AttackEmitter hammerAttack;
    public AttackEmitter dashAttack;
    HealthModule         _healthModule;
    NavMeshMovement      _movementModule;
    Transform            _target;
    float                _dashCooldownTimer;
    float                _slamCooldownTimer;
    Action               _attackEvent;
    Action<float>        _onTakeDamage;
    protected override BurgerBotState InitialState => Idle;
    protected override BurgerBotSm Context => this;

    [SerializeField] BurgerBotState state;

    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }
    void HammerAttack()
    {
        animator.SetTrigger(_hAttackTrigger);
        animator.SetTrigger(_hMarteladaTrigger);
    }
    void OnTakeDamage(float dmg)
    {
        if (hitFeedback && dmg > 0) hitFeedback.PlayFeedbacks();
        if (_healthModule.Health <= 0) explosionFeedback.PlayFeedbacks();
    }
    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        _healthModule = Character.GetModule<HealthModule>();
        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;
        _attackEvent = HammerAttack;
        _onTakeDamage = OnTakeDamage;
        IdleState();
        HammerSlamState();
        DashState();
        ChasingState();
        DyingState();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        hammerAttack.AddListener(_attackEvent);
        _healthModule.OnTakeDamage += _onTakeDamage;
    }
    void OnDisable()
    {
        hammerAttack.RemoveListener(_attackEvent);
        _healthModule.OnTakeDamage -= _onTakeDamage;
    }
    protected override void Update()
    {
        state = stateMachine.CurrentState;
        base.Update();
        _dashCooldownTimer = Math.Max(_dashCooldownTimer - Time.deltaTime, 0);
        _slamCooldownTimer = Math.Max(_slamCooldownTimer - Time.deltaTime, 0);
        
        if (_healthModule.Health <= _healthModule.MaxHealth * 0.25f) {
            lowLifeFeedback.PlayFeedbacks();
        }
    }
    void DyingState()
    {
        stateMachine.Transition(Dying, static sm => sm._healthModule.Health <= 0);
        stateMachine.From(Dying).AddListener(LifeCycle.Enter, static sm => {
            if (sm.explosionFeedback) {
                //TODO: fix this mother of all hacks
                var t = sm.transform;
                var scale = t.localScale;
                var position = t.position;
                sm.explosionFeedback.PlayFeedbacks(position);
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
    void IdleState()
    {
        stateMachine.From(Idle)
            .Transition(Dash, DashPredicate)
            .Transition(Chasing, sm => sm._movementModule.Target)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .AddListener(LifeCycle.Enter, sm => {
                sm.animator.SetBool(sm._hIdleBool, true);
            })
            .AddListener(LifeCycle.Exit, sm => {
                sm.animator.SetBool(sm._hIdleBool, false);
            });
    }
    void DashState()
    {
        stateMachine.From(Dash)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .AddListener(LifeCycle.Enter, sm => {
                TurnToTarget(sm);
                sm.dashAttack.enabled = true;
                sm.animator.SetTrigger(sm._hAttackTrigger);
                sm.animator.SetTrigger(sm._hDashTrigger);
                sm.Delay(Time.deltaTime * 33.34f, sm => sm._movementModule.StartDash());
            })
            .AddListener(LifeCycle.Exit, sm => {
                sm.dashAttack.enabled = false;
                sm._movementModule.DashTween.Kill();
                sm._dashCooldownTimer = UnityEngine.Random.Range(sm.dashCooldown.x, sm.dashCooldown.y);
            });
    }
    void HammerSlamState()
    {
        stateMachine.From(HammerSlam)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Chasing, sm => !HammerSlamRangePredicate(sm))
            .AddListener(LifeCycle.Enter, sm => {
                sm.animator.SetTrigger(sm._hAttackTrigger);
                sm.animator.SetTrigger(sm._hMarteladaTrigger);
                sm.hammerAttack.enabled = true;
            })
            .AddListener(LifeCycle.Exit, sm => {
                sm.hammerAttack.enabled = false;
                sm._slamCooldownTimer = sm.slamCooldown;
            })
            .AddListener(LifeCycle.Update, TurnToTarget);
    }
    static void TurnToTarget(BurgerBotSm sm)
    {
        var smTransform = sm.transform;
        var targetPos = sm._target.position;
        var position = smTransform.position;
        var direction = Vector3Ext.Direction(position, targetPos);
        var rotation = Quaternion.LookRotation(direction);
        rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        smTransform.rotation = rotation;
    }
    void ChasingState()
    {
        stateMachine.From(Chasing)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Dash, DashPredicate)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .AddListener(LifeCycle.Enter, sm => {
                sm.animator.SetBool(sm._hWalkBool, true);
                sm._movementModule.StartChase();
            })
            .AddListener(LifeCycle.Exit, sm => {
                sm.animator.SetBool(sm._hWalkBool, false);
                sm._movementModule.StopMovement();
            });
    }
    static bool HammerSlamRangePredicate(BurgerBotSm sm)
    {
        if (!sm._target) return false;
        var targetPos = sm._target.position;
        var position = sm.transform.position;
        bool canSlam = sm._slamCooldownTimer <= 0;
        float distSqr = Vector3Ext.SqrDistance(position, targetPos);
        float rangeTip = sm.slamRange;
        return canSlam & distSqr < Mathf.Pow(rangeTip, 2);
    }
    static bool DashPredicate(BurgerBotSm sm)
    {
        bool canDash = sm._dashCooldownTimer <= 0 && sm.dashAttack;
        bool outOfRange = !HammerSlamRangePredicate(sm);
        bool hasTarget = sm._movementModule.Target;
        return canDash & hasTarget & outOfRange;
    }
    void OnCollisionEnter(Collision collision)
    {
        bool witWall = collision.gameObject.CompareTag("Wall");
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
    Dying
}