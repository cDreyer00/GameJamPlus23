using System;
using System.Collections;
using System.Collections.Generic;
using Sources.Characters.Modules;
using UnityEngine;
using UnityEngine.AI;
using Sources.Systems.FSM;
using Unity.VisualScripting;
using UnityEditor;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    bool _registered;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform target;
    [SerializeField] float startCooldownTime;
    public NavMeshAgent Agent => agent;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    void Start()
    {
        if (Character.TryGetModule<StateModule>(out var stateModule))
        {
            if (!_registered)
            {
                var fsm = stateModule.StateMachine;
                fsm.Transition(Character.State.Idle, () => target == null);
                fsm.Transition(Character.State.Idle, Character.State.Chasing, () => target != null);
                fsm[LifeCycle.Enter, Character.State.Chasing] += () => agent.isStopped = false;
                fsm[LifeCycle.Exit, Character.State.Chasing] += () => agent.isStopped = true;
                fsm[LifeCycle.Update, Character.State.Chasing] += () => SetDestination(target.position);
                Helpers.Delay(startCooldownTime, static self => self.SetTarget(GameManager.Instance.Player.transform), this);
                _registered = true;
            }
        }
        else
        {
            Debug.LogWarning("NavMeshMovement: StateModule not found, using default behavior.");
            Helpers.Delay(startCooldownTime, static self => self.SetTarget(GameManager.Instance.Player.transform), this);
            Helpers.Repeat(
                startCooldownTime + Time.fixedDeltaTime,
                Time.fixedDeltaTime,
                static self => self.SetDestination(self.target.position),
                this
            );
        }
    }
    protected override void Init()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        Debug.Log("enemy freeze subscribed");
        Character.Events.freeze += Freeze;
    }
    public void SetDestination(Vector3 position)
    {
        if (this.IsDestroyed() || !gameObject.activeInHierarchy) return;
        agent.SetDestination(position);
    }
    void Freeze(float duration)
    {
        agent.isStopped = true;
        Helpers.Delay(duration, () =>
        {
            if (this.IsDestroyed() || !gameObject.activeInHierarchy) return;
            agent.isStopped = false;
        });
    }
}