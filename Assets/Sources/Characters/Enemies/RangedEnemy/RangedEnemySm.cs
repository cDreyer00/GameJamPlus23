using System;
using MoreMountains.Feedbacks;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

public class RangedEnemySm : StateMachineModule<RangedEnemySm, Character.State>
{
    HealthModule                 _healthModule;
    ProjectileLauncher           _attackModule;
    [SerializeField] MMFeedbacks hitFeedBack;
    [SerializeField] MMFeedbacks explosionFeedback;
    protected override Character.State InitialState => Idle;
    protected override RangedEnemySm Context => this;
    Action<float> _onTakeDamage;
    void OnTakeDamage(float dmg)
    {
        if (hitFeedBack && dmg > 0) hitFeedBack.PlayFeedbacks();
        if (_healthModule.Health <= 0) explosionFeedback.PlayFeedbacks();
    }
    protected override void Init()
    {
        base.Init();
        _attackModule = Character.GetModule<ProjectileLauncher>();
        _attackModule.Target = GameManager.Instance.Player.transform;
        _healthModule = Character.GetModule<HealthModule>();
        _onTakeDamage = OnTakeDamage;
        IdleState();
        AttackingState();
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
    void DyingState()
    {
        stateMachine.Transition(Dying, static sm => sm._healthModule.Health <= 0);
        stateMachine.From(Dying).AddListener(LifeCycle.Enter, static sm => {
            if (sm.explosionFeedback) sm.explosionFeedback.PlayFeedbacks();
            var character = sm.Character;
            character.Events.Died(character);
        });
    }
    void IdleState()
    {
        stateMachine.From(Idle).Transition(Attacking, static sm => sm._attackModule.Target);
    }
    void AttackingState()
    {
        stateMachine.From(Attacking)
            .Transition(Idle, static sm => !sm._attackModule.Target)
            .AddListener(LifeCycle.Enter, static sm => sm._attackModule.StartModule())
            .AddListener(LifeCycle.Exit, static sm => sm._attackModule.StopModule());
    }
}