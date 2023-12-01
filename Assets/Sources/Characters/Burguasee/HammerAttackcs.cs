using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using Sources;
using Sources.Characters.Modules;
using Sources.Projectile;
using UnityEngine;
using UnityEngine.Serialization;

public class HammerAttack : CharacterModule
{
    [SerializeField] float        lifeTime;
    [SerializeField] float        delay;
    [SerializeField] ImpactDamage attackPointPrefab;

    GameObject _attackPoint;
    Transform  _target;
    public Transform Target
    {
        get => _target;
        set => _target = value;
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
    public void MoveAttackPoint(Vector3 position)
    {
        if (!_attackPoint) {
            _attackPoint = Instantiate(attackPointPrefab.gameObject, transform.position, Quaternion.identity);
        }
        _attackPoint.transform.position = position;
    }
    void Attack()
    {
        _attackPoint = _attackPoint.OrNull() ?? Instantiate(attackPointPrefab.gameObject, transform.position, Quaternion.identity);
        var dc = _attackPoint.GetComponent<ImpactDamage>();
        dc.IgnoreTeam(Character.team);
        _attackPoint.SetActive(false);

        this.Delay(delay, static s => s._attackPoint.SetActive(true));
        this.Delay(0.1f, static s => s._attackPoint.SetActive(false));
    }
    protected override void Init() {}
}