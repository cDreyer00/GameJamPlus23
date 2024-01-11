using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AttackEmitter : MonoBehaviour, IEventEmitter<Action>
{
    IEnumerator              _emmit;
    Action<Object, Collider> _onCollision;
    public List<string>      ignoreList;
    public float             cooldown;
    public float             damage;
    public CollisionEmitter  collisionEmitter;
    event Action EventNoArgs;
    event Action<Object, object> Event;
    public void AddListener(Action action)
    {
        EventNoArgs += action;
    }
    public void RemoveListener(Action action)
    {
        EventNoArgs -= action;
    }
    public void IgnoreTeam(string team)
    {
        if (!ignoreList.Contains(team))
            ignoreList.Add(team);
    }
    IEnumerator EmmitCoroutine()
    {
        while (true) {
            if (cooldown == 0) {
                yield return null;
            }
            else {
                yield return Helpers.GetWait(cooldown);
            }
            EventNoArgs?.Invoke();
            Event?.Invoke(this, null);
        }
    }
    void Awake()
    {
        _onCollision = OnCollision;
        _emmit = EmmitCoroutine();
    }
    void OnValidate()
    {
        if (!collisionEmitter) collisionEmitter = GetComponentInChildren<CollisionEmitter>();
    }
    void OnEnable()
    {
        collisionEmitter.enabled = true;
        StartCoroutine(_emmit);
        collisionEmitter.AddListener(_onCollision);
    }
    void OnDisable()
    {
        collisionEmitter.enabled = false;
        collisionEmitter.RemoveListener(_onCollision);
    }
    void OnCollision(Object sender, Collider col)
    {
        var targetCharacter = col.GetComponent<Character>();
        if (!targetCharacter) return;
        if (ignoreList.Contains(targetCharacter.team)) return;
        targetCharacter.Events.TakeDamage(damage);
    }
}