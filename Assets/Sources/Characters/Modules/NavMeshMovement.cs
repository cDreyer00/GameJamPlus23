using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Transform    target;
    [SerializeField] float        startCooldownTime;
    public NavMeshAgent Agent => agent;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public override void StartModule()
    {
        agent.isStopped = false;
        InvokeRepeating(nameof(Chase), startCooldownTime, 0.1f);
    }
    public override void StopModule()
    {
        CancelInvoke(nameof(Chase));
        agent.isStopped = true;
    }
    protected override void Init()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        Debug.Log("enemy freeze subscribed");
        Character.Events.Freeze += OnFreeze;
    }
    void Chase()
    {
        if (!agent.isOnNavMesh) return;
        agent.SetDestination(target.position);
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