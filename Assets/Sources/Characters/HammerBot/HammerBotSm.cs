using System;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static HammerBotState;

public class HammerBotSm : StateMachineModule<HammerBotSm, HammerBotState>
{
    [SerializeField] float dashCooldown;
    [SerializeField] float slamCooldown;
    [SerializeField] bool  dashEnabled;

    NavMeshMovement _movementModule;
    HammerAttack    _hammerAttack;
    Transform       _target;
    protected override HammerBotState InitialState => Idle;
    protected override HammerBotSm Context => this;
    protected override void Init()
    {
        base.Init();

        _movementModule = Character.GetModule<NavMeshMovement>();
        _hammerAttack = Character.GetModule<HammerAttack>();

        _target = GameManager.Instance.Player.transform;
        _movementModule.Target = _target;
        _hammerAttack.Target = _target;
    }
}

public enum HammerBotState
{
    Idle,
    Dash,
    Slam,
    Chasing,
}