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
    public   float        damage;
    public   ImpactDamage impactDamage;
    event Action<Character> AttackCallback;
    void OnValidate()
    {
        if (!impactDamage) impactDamage = GetComponentInChildren<ImpactDamage>();
        impactDamage.gameObject.SetActive(false);
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
        impactDamage.gameObject.SetActive(false);
        StartCoroutine(_attackCoroutine);
    }
    public virtual void StopAttack()
    {
        StopCoroutine(_attackCoroutine);
        impactDamage.gameObject.SetActive(false);
    }
    void IgnoreTeam(string team)
    {
        if (!_ignoreList.Contains(team))
            _ignoreList.Add(team);
    }
    IEnumerator AttackCoroutine()
    {
        impactDamage.gameObject.SetActive(false);

        while (true) {
            AttackCallback?.Invoke(Character);
            impactDamage.gameObject.SetActive(true);
            yield return null;
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