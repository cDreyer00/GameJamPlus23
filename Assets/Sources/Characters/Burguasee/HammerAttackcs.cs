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
    public float        lifeTime;
    public float        delay;
    public ImpactDamage impactDamage;
    public Transform ImpactPoint => impactDamage.transform;

    IEnumerator _attackCoroutine;
    void Start()
    {
        _attackCoroutine = AttackCoroutine();
    }
    public virtual void StartAttack()
    {
        impactDamage.gameObject.SetActive(false);
        StartCoroutine(_attackCoroutine);
    }
    public virtual void StopAttack()
    {
        StopCoroutine(_attackCoroutine);
        impactDamage.gameObject.SetActive(false);
    }
    IEnumerator AttackCoroutine()
    {
        impactDamage.IgnoreTeam(Character.team);
        impactDamage.gameObject.SetActive(false);

        while (true) {
            yield return Helpers.GetWait(delay);
            impactDamage.gameObject.SetActive(true);
            yield return Helpers.GetWait(lifeTime);
            impactDamage.gameObject.SetActive(false);
        }
    }
    protected override void Init() {}
}