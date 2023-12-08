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

public class AttackEventEmitter : MonoBehaviour
{
    IEnumerator             _attackCoroutine;
    public List<string>     ignoreList;
    public float            cooldown;
    public float            damage;
    public ColliderCallback colliderCallback;
    event Action AttackCallback;
    public void AddListener(Action action)
    {
        AttackCallback += action;
    }
    public void RemoveListener(Action action)
    {
        AttackCallback -= action;
    }
    public virtual void StartAttack()
    {
        colliderCallback.enabled = true;
        StartCoroutine(_attackCoroutine);
    }
    public virtual void StopAttack()
    {
        StopCoroutine(_attackCoroutine);
        colliderCallback.enabled = false;
    }
    public void IgnoreTeam(string team)
    {
        if (!ignoreList.Contains(team))
            ignoreList.Add(team);
    }
    IEnumerator AttackCoroutine()
    {
        while (true) {
            if (cooldown == 0) {
                yield return null;
            }
            else {
                yield return Helpers.GetWait(cooldown);
            }
            AttackCallback?.Invoke();
        }
    }
    void Awake()
    {
        colliderCallback.enabled = false;
        _attackCoroutine = AttackCoroutine();
        colliderCallback.AddListener(OnImpact);

        void OnImpact(Collider col)
        {
            var targetCharacter = col.GetComponent<Character>();
            if (!targetCharacter) return;
            if (ignoreList.Contains(targetCharacter.team)) return;
            targetCharacter.Events.TakeDamage(damage);
        }
    }
}