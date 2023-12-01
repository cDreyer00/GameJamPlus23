using System;
using System.Collections;
using System.Collections.Generic;
using Sources;
using Sources.Characters.Modules;
using Sources.Projectile;
using UnityEngine;
using UnityEngine.Serialization;

public class HammerAttack : CharacterModule
{
    [SerializeField] float        lifeTime;
    [SerializeField] float        collapsed;
    [SerializeField] ImpactDamage attackPointPrefab;

    GameObject _attackPoint;
    Transform  _target;
    float      _attackRange;
    public Transform Target
    {
        get => _target;
        set => _target = value;
    }
    public float AttackRange
    {
        get => _attackRange;
        set => _attackRange = value;
    }
    public override void StartModule()
    {
        InvokeRepeating(nameof(Attack), 0, lifeTime);
    }
    public override void StopModule()
    {
        CancelInvoke(nameof(Attack));
        _attackPoint.SetActive(false);
    }
    void Attack()
    {
        var t         = transform;
        var targetPos = _target.position;
        var pos       = t.position;
        var dir       = (targetPos - pos).normalized;
        _attackPoint = _attackPoint.OrNull() ?? Instantiate(attackPointPrefab.gameObject, Vector3.zero, Quaternion.identity);
        var dc = _attackPoint.GetComponent<ImpactDamage>();
        dc.IgnoreTeam(Character.team);
        _attackPoint.SetActive(false);
        var attPos = (pos + dir * _attackRange).With(y: pos.y + targetPos.y / 2);
        _attackPoint.transform.position = attPos;
        Invoke(nameof(BeginCollapse), collapsed);
    }
    void BeginCollapse()
    {
        _attackPoint.SetActive(true);
        Invoke(nameof(EndCollapse), 0.1f);
    }
    void EndCollapse()
    {
        _attackPoint.SetActive(false);
    }
    protected override void Init() {}
}