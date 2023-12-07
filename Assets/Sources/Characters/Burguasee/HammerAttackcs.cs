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
    IEnumerator           _attackCoroutine;
    readonly List<string> _ignoreList = new();
    public   float        cooldown;
    public   float        damage;
    public   ImpactDamage impactDamage;
    event Action<Character> AttackCallback;
    void OnValidate()
    {
        if (!impactDamage) impactDamage = GetComponentInChildren<ImpactDamage>();
    }
    void Start()
    {
        _attackCoroutine = AttackCoroutine();
    }
    public void AddListener(Action<Character> action)
    {
        AttackCallback += action;
    }
    public void RemoveListener(Action<Character> action)
    {
        AttackCallback -= action;
    }
    public virtual void StartAttack()
    {
        StartCoroutine(_attackCoroutine);
    }
    public virtual void StopAttack()
    {
        StopCoroutine(_attackCoroutine);
    }
    void IgnoreTeam(string team)
    {
        if (!_ignoreList.Contains(team))
            _ignoreList.Add(team);
    }
    IEnumerator AttackCoroutine()
    {
        while (true) {
            yield return Helpers.GetWait(cooldown);
            AttackCallback?.Invoke(Character);
        }
    }
    protected override void Init()
    {
        IgnoreTeam(Character.team);
        impactDamage.AddListener(OnImpact);

        void OnImpact(Collider col)
        {
            var targetCharacter = col.GetComponent<Character>();
            if (!targetCharacter) return;
            if (_ignoreList.Contains(targetCharacter.team)) return;
            targetCharacter.Events.TakeDamage(damage);
        }
    }
}