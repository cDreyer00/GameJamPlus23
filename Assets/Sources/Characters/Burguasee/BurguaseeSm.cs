using System;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static HammerBotState;

public class BurguaseeSm : StateMachineModule<BurguaseeSm, HammerBotState>
{
    [SerializeField] float slamRange;
    [SerializeField] float dashCooldown;
    [SerializeField] float slamCooldown;
    [SerializeField] bool  dashEnabled;

    NavMeshMovement _movementModule;
    HammerAttack    _hammerAttack;
    Transform       _target;
    float           _dashCooldownTimer;
    float           _slamCooldownTimer;
    protected override HammerBotState InitialState => Idle;
    protected override BurguaseeSm Context => this;
    protected override void Init()
    {
        base.Init();

        _movementModule = Character.GetModule<NavMeshMovement>();
        _hammerAttack = Character.GetModule<HammerAttack>();
        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;
        _hammerAttack.Target = _target;

        IdleState();
        SlamState();
        DashState();
        ChasingState();
    }

    void IdleState()
    {
        stateMachine.Transition(Idle, Dash, DashPredicate);
        stateMachine.Transition(Idle, Chasing, static sm => sm._movementModule.Target);
        stateMachine.Transition(Idle, Slam, SlamPredicate);
    }
    void DashState()
    {
        stateMachine.Transition(Dash, Idle, static sm => !sm._movementModule.Target);
        stateMachine.Transition(Dash, Chasing, static sm => sm._movementModule.Agent.velocity.sqrMagnitude <= 0.1f);

        stateMachine[LifeCycle.Enter, Dash] = static sm => {
            sm._movementModule.StartModule();
            sm._movementModule.Agent.speed *= 2;
            sm._movementModule.Agent.angularSpeed *= 2;
            sm._movementModule.Agent.acceleration *= 2;
        };
        stateMachine[LifeCycle.Exit, Dash] = static sm => {
            sm._movementModule.StopModule();
            sm._movementModule.Agent.speed /= 2;
            sm._movementModule.Agent.angularSpeed /= 2;
            sm._movementModule.Agent.acceleration /= 2;
            sm._dashCooldownTimer = sm.dashCooldown;
        };
    }
    void SlamState()
    {
        stateMachine.Transition(Slam, Idle, static sm => !sm._movementModule.Target);
        stateMachine.Transition(Slam, Chasing, static sm => {
            float distSqr     = (sm._movementModule.Target.position - sm.Character.transform.position).sqrMagnitude;
            float slamDistSqr = sm.slamRange * sm.slamRange;
            bool  inRange     = distSqr <= slamDistSqr;
            bool  inCooldown  = sm._slamCooldownTimer > 0;
            return !inRange || inCooldown;
        });

        stateMachine[LifeCycle.Enter, Slam] = static sm => {
            sm._hammerAttack.MoveAttackPoint(sm._target.position);
            sm._hammerAttack.StartModule();
        };
        stateMachine[LifeCycle.Exit, Slam] = static sm => {
            sm._hammerAttack.StopModule();
            sm._slamCooldownTimer = sm.slamCooldown;
        };
    }
    void ChasingState()
    {
        stateMachine.Transition(Chasing, Idle, static sm => !sm._movementModule.Target);
        stateMachine.Transition(Chasing, Dash, DashPredicate);
        stateMachine.Transition(Chasing, Slam, SlamPredicate);
        stateMachine[LifeCycle.Enter, Chasing] = static sm => sm._movementModule.StartModule();
        stateMachine[LifeCycle.Exit, Chasing] = static sm => sm._movementModule.StopModule();
    }
    static bool SlamPredicate(BurguaseeSm sm)
    {
        bool  couldSlam   = sm._hammerAttack.Target;
        float slamDistSqr = sm.slamRange * sm.slamRange;
        float distSqr     = (sm._hammerAttack.Target.position - sm.Character.transform.position).sqrMagnitude;
        return couldSlam && distSqr <= slamDistSqr;
    }
    static bool DashPredicate(BurguaseeSm sm)
    {
        bool couldDash  = sm.dashEnabled && sm._movementModule.Target;
        bool shouldDash = UnityEngine.Random.Range(0, 100) <= 33;
        bool onCooldown = sm._dashCooldownTimer > 0;
        return couldDash && shouldDash && !onCooldown;
    }
}

public enum HammerBotState
{
    Idle,
    Dash,
    Slam,
    Chasing,
}