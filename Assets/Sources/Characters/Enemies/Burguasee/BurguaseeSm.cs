using System;
using Sources.Systems.FSM;
using UnityEngine;
using static BorguaseState;
using Object = UnityEngine.Object;

public class BurguaseeSm : StateMachineModule<BurguaseeSm, BorguaseState>
{
    [SerializeField] float     slamRange;
    [SerializeField] Vector2   dashCooldown;
    [SerializeField] Animator  animator;
    [SerializeField] Character nextPhase;
    [SerializeField] float     poolCollectDelay = 1f;

    readonly int _hAttackTrigger = Animator.StringToHash("attack");
    readonly int _hDeadTrigger   = Animator.StringToHash("isDead");
    readonly int _hWalkBool      = Animator.StringToHash("isWalk");

    public AttackEmitter hammerAttack;

    Transform       _target;
    NavMeshMovement _movementModule;
    HealthModule    _healthModule;
    protected override BorguaseState InitialState => Idle;
    protected override BurguaseeSm Context => this;

    [SerializeField] BorguaseState state;

    Action _attackEvent;
    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }
    protected override void Init()
    {
        base.Init();
        _attackEvent = HammerAttack;
        _movementModule = Character.GetModule<NavMeshMovement>();
        _healthModule = Character.GetModule<HealthModule>();
        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;

        IdleState();
        HammerSlamState();
        ChasingState();
        DyingState();
    }
    void OnDisable()
    {
        hammerAttack.RemoveListener(_attackEvent);
        hammerAttack.enabled = false;
    }
    void HammerAttack()
    {
        animator.SetTrigger(_hAttackTrigger);
    }
    protected override void Update()
    {
        base.Update();
        state = stateMachine.CurrentState;
    }
    void DyingState()
    {
        stateMachine.Transition(Dying, static sm => sm._healthModule.Health <= 0);
        stateMachine.From(Dying).AddListener(LifeCycle.Enter, static sm => {
            if (sm.animator) {
                sm.animator.SetBool(sm._hWalkBool, false);
                sm.animator.SetTrigger(sm._hDeadTrigger);
            }
            sm.Delay(sm.poolCollectDelay, static sm => {
                sm.hammerAttack.enabled = false;
                sm.hammerAttack.RemoveListener(sm._attackEvent);
                var t = sm.transform;
                sm.Character.Events.Died(sm.Character);
                if (sm.nextPhase) Instantiate(sm.nextPhase, t.position, t.rotation);
            });
        });
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
            .AddListener(LifeCycle.Enter, sm => {
                sm.hammerAttack.AddListener(sm._attackEvent);
                sm.hammerAttack.enabled = true;
            })
            .AddListener(LifeCycle.Exit, sm => {
                sm.hammerAttack.RemoveListener(sm._attackEvent);
            })
            .AddListener(LifeCycle.Update, sm => {
                var smTransform = sm.transform;
                var targetPos = sm._target.position;
                var position = smTransform.position;
                var direction = Vector3Ext.Direction(position, targetPos);
                var rotation = Quaternion.LookRotation(direction);
                rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
                smTransform.rotation = rotation;
            });
    }
    void ChasingState()
    {
        stateMachine.From(Chasing)
            .Transition(Idle, sm => !sm._movementModule.Target)
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
    static bool HammerSlamRangePredicate(BurguaseeSm sm)
    {
        if (!sm._target) return false;
        var targetPos = sm._target.position;
        var position = sm.transform.position;
        float distSqr = Vector3Ext.SqrDistance(position, targetPos);
        float rangeTip = sm.slamRange;
        return distSqr < Mathf.Pow(rangeTip, 2);
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
    Dying
}