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
    public   float        lifeTime;
    public   float        delay;
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
    public void SetListener(Action<Character> action)
    {
        if (AttackCallback != null) {
            Debug.LogWarning("HammerAttack already has a listener, Use AddListener instead.");
            return;
        }
        AttackCallback = action;
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
            yield return Helpers.GetWait(delay);
            AttackCallback?.Invoke(Character);
            impactDamage.gameObject.SetActive(true);
            yield return Helpers.GetWait(lifeTime);
            impactDamage.gameObject.SetActive(false);
        }
    }
    protected override void Init()
    {
        IgnoreTeam(Character.team);
        impactDamage.SetListener(OnImpact);

        void OnImpact(Collider col)
        {
            var targetCharacter = col.GetComponent<Character>();
            if (!targetCharacter) return;
            if (_ignoreList.Contains(targetCharacter.team)) return;
            targetCharacter.Events.TakeDamage(damage);
        }
    }
}