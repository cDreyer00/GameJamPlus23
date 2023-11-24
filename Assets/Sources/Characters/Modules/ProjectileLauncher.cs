using System;
using System.Collections;
using System.Collections.Generic;
using Sources.Characters.Modules;
using Sources.Systems.FSM;
using UnityEngine;
using UnityEngine.Serialization;
using static Character.State;

public class ProjectileLauncher : StateModule
{
    QueuePool<Projectile> _projectilePool;

    [SerializeField] Projectile projectile;
    [SerializeField] float      delay;
    [SerializeField] float      coolDown;

    Character _target;
    bool      _isAttacking;
    protected override void Init()
    {
        base.Init();
        _projectilePool = new QueuePool<Projectile>(projectile, 10);
        ToState(Idle, () => _isAttacking || !_target);
        FromState(Attacking, () => !_isAttacking && _target);
        Helpers.Delay(delay, () => _target = GameManager.Instance.Player);
    }
    public override Character.State StateEnum => Attacking;
    public override void Enter()
    {
        _isAttacking = true;
        var dir = (_target.Position - Character.Position).normalized;
        var p   = _projectilePool.Get(Character.Position, Quaternion.LookRotation(dir));
        p.IgnoreTeam(Character.team);
        p.target = _target.Position;
        Helpers.Delay(coolDown, () => _isAttacking = false);
    }
    public override void FixedUpdate() {}
    public override void Update() {}
    public override void Exit() {}
}