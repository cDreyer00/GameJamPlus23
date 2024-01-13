using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class AttackEmitter : MonoBehaviour, IEventEmitter<Action>
{
//<<<<<<< HEAD
    IEnumerator              _emmit;
    Action<Object, Collider> _onCollision;
    public List<string>      ignoreList;
    public float             cooldown;
    public float             damage;
    public CollisionEmitter  collisionEmitter;
    event Action EventNoArgs;
    event Action<Object, object> Event;
    [SerializeField] AudioClip attackAudio;
// =======
//
//     IEnumerator             _attackCoroutine;
//     public List<string>     ignoreList;
//     public float            cooldown;
//     public float            damage;
//     public ColliderCallback colliderCallback;
//     event Action AttackCallback;
// >>>>>>> 27b73a94e1852dba9c401135c90eb2ed5c6bae1b
    public void AddListener(Action action)
    {
        EventNoArgs += action;
    }
    public void RemoveListener(Action action)
    {
//<<<<<<< HEAD
        EventNoArgs -= action;
// =======
//         AttackCallback -= action;
    }
    public virtual void StartAttack()
    {
        collisionEmitter.enabled = true;
        StartCoroutine(_emmit);

        // if(attackAudio)
        //     attackAudio.Play();
    }
    public virtual void StopAttack()
    {
        StopCoroutine(_emmit);
        collisionEmitter.enabled = false;
//>>>>>>> 27b73a94e1852dba9c401135c90eb2ed5c6bae1b
    }
    public void IgnoreTeam(string team)
    {
        if (!ignoreList.Contains(team))
            ignoreList.Add(team);
    }
    IEnumerator EmmitCoroutine()
    {
        while (true) {
            EventNoArgs?.Invoke();
            Event?.Invoke(this, null);
            if (cooldown == 0) {
                yield return null;
            }
            else {
                yield return Helpers.GetWait(cooldown);
            }
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