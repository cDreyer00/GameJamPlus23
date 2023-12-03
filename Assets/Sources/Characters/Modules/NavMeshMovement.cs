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

[RequireComponent(typeof(NavMeshAgent)), Serializable]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] float startCooldownTime;
    [SerializeField] float dashForce;

    Rigidbody _rigidbody;
    public NavMeshAgent Agent => agent;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    IEnumerator _chaseCoroutine;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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
        if (_rigidbody)
        {
            _rigidbody.velocity = Vector3.zero;
            //_rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.AddForce(transform.forward * dashForce, ForceMode.Impulse);
        }
    }
    public void StopMovement()
    {
        agent.isStopped = true;
        StopCoroutine(_chaseCoroutine);

        if (_rigidbody)
        {
            _rigidbody.velocity = Vector3.zero;
            //_rigidbody.angularVelocity = Vector3.zero;
        }
    }
    protected override void Init()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        Character.Events.Freeze += OnFreeze;
    }
    IEnumerator ChaseCoroutine()
    {
        while (!agent.isStopped)
        {
            agent.SetDestination(target.position);
            yield return null;
        }
    }
    void OnFreeze(float duration)
    {
        agent.isStopped = true;
        Helpers.Delay(duration, () =>
        {
            if (this.IsDestroyed() || !this.gameObject.activeInHierarchy) return;
            this.agent.isStopped = false;
        });
    }
}