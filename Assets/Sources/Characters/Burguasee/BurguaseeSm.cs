using System;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static BorguaseState;

public class BurguaseeSm : StateMachineModule<BurguaseeSm, BorguaseState>
{
    [SerializeField] float slamRange;
    [SerializeField] float dashCooldown;
    [SerializeField] float slamCooldown;

    [SerializeField, Range(0f, 1f)] float dashChance;

    NavMeshMovement _movementModule;
    HammerAttack    _hammerAttack;
    Transform       _target;
    float           _dashCooldownTimer;
    float           _slamCooldownTimer;
    protected override BorguaseState InitialState => Idle;
    protected override BurguaseeSm Context => this;

    [SerializeField] BorguaseState state;
    protected override void Init()
    {
        base.Init();
        _movementModule = Character.GetModule<NavMeshMovement>();
        _hammerAttack = Character.GetModule<HammerAttack>();
        _hammerAttack.ImpactPoint.SetLocalPosition(z: slamRange);
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
            .Transition(HammerSlam, HammerSlamRangePredicate);
    }
    void DashState()
    {
        stateMachine.From(Dash)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .SetCallback(LifeCycle.Enter, sm => sm._movementModule.StartDash())
            .SetCallback(LifeCycle.Exit, sm => {
                sm._movementModule.StopMovement();
                sm._dashCooldownTimer = sm.dashCooldown;
            });
    }
    void HammerSlamState()
    {
        stateMachine.From(HammerSlam)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Chasing, sm => !HammerSlamRangePredicate(sm))
            .SetCallback(LifeCycle.Enter, sm => {
                sm.transform.LookAt(sm._target);
                sm._hammerAttack.StartAttack();
            })
            .SetCallback(LifeCycle.Exit, sm => {
                sm._hammerAttack.StopAttack();
                sm._slamCooldownTimer = sm.slamCooldown;
            });
    }
    void ChasingState()
    {
        stateMachine.From(Chasing)
            .Transition(Idle, sm => !sm._movementModule.Target)
            .Transition(Dash, DashPredicate)
            .Transition(HammerSlam, HammerSlamRangePredicate)
            .SetCallback(LifeCycle.Enter, sm => sm._movementModule.StartChase())
            .SetCallback(LifeCycle.Exit, sm => sm._movementModule.StopMovement());
    }
    static bool HammerSlamRangePredicate(BurguaseeSm sm)
    {
        var   targetPos = sm._target.position;
        var   position  = sm.transform.position;
        bool  canSlam   = sm._slamCooldownTimer <= 0;
        float distSqr   = Vector3Ext.SqrDistance(position, targetPos);
        return canSlam && distSqr < sm.slamRange * sm.slamRange;
    }
    static bool DashPredicate(BurguaseeSm sm)
    {
        bool canDash    = sm._dashCooldownTimer <= 0;
        bool shouldDash = UnityEngine.Random.Range(0f, 1f) < sm.dashChance;
        return canDash && shouldDash && sm._movementModule.Target && !HammerSlamRangePredicate(sm);
    }
    void OnCollisionEnter(Collision collision)
    {
        bool hitPlayer = collision.gameObject.CompareTag("Player");

        if (stateMachine.CurrentState == Dash) {
            if (hitPlayer) {
                slamCooldown = 0;
                stateMachine.ChangeState(HammerSlam);
            }
            stateMachine.ChangeState(Chasing);
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Rigidbody rb)) {
            rb.velocity *= 0.1f;
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