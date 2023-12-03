using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Sources.Characters.Modules;
using UnityEngine;
using UnityEngine.AI;
using Sources.Systems.FSM;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshAgent)), Serializable]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform    target;
    [SerializeField] float        dashDistance;
    [SerializeField] float        dashDuration;
    [SerializeField] Ease         dashEase;
    public NavMeshAgent Agent => agent;
    public Tween dashTween;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    IEnumerator _chaseCoroutine;
    void Start()
    {
        _chaseCoroutine = ChaseCoroutine();
    }
    public void StartChase()
    {
        agent.isStopped = false;
        StartCoroutine(_chaseCoroutine);
    }
    public void StartDash()
    {
        agent.isStopped = false;

        agent.transform.LookAt(target);
        var direction = Vector3Ext.Direction(transform.position, target.position);
        dashTween = agent.transform.DOMove(direction * dashDistance, dashDuration).SetEase(dashEase);
    }
    void OnCollisionEnter(Collision collision)
    {
        bool hitWall = collision.gameObject.CompareTag("Wall");
        if (hitWall) dashTween?.Kill();
    }
    public void StopMovement()
    {
        agent.isStopped = true;
        StopCoroutine(_chaseCoroutine);
    }
    protected override void Init()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        Character.Events.OnFreeze += OnFreeze;
    }
    IEnumerator ChaseCoroutine()
    {
        while (!agent.isStopped) {
            agent.SetDestination(target.position);
            yield return null;
        }
    }
    void OnFreeze(float duration)
    {
        agent.isStopped = true;
        this.Delay(duration, static c => {
            if (c.IsDestroyed() || !c.gameObject.activeInHierarchy) return;
            c.agent.isStopped = false;
        });
    }
}