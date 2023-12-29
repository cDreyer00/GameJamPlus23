using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEditor;
using MoreMountains.Feedbacks;
using Sources;
using Object = UnityEngine.Object;

[RequireComponent(typeof(NavMeshAgent)), Serializable]
public class NavMeshMovement : CharacterModule, IMovementModule
{
    [SerializeField] NavMeshAgent          agent;
    [SerializeField] Transform             target;
    [SerializeField] float                 dashDistance;
    [SerializeField] float                 dashDuration;
    [SerializeField] Ease                  dashEase;
    [SerializeField] MMFeedbacks           freezeFeedback;
    [SerializeField] ScriptableObjectEvent onPause;

    public NavMeshAgent Agent => agent;
    public Tween DashTween;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    IEnumerator   _chaseCoroutine;
    Action        _toggleMovement;
    Action<float> _freeze;
    void ToggleMovement() => agent.isStopped = !agent.isStopped;
    void OnValidate()
    {
        if (!agent) agent = GetComponentInChildren<NavMeshAgent>();
        if (!onPause) onPause = GameEvents.onPause;
    }
    void OnEnable()
    {
        Character.Events.OnFreeze += _freeze;
        onPause.AddListener(_toggleMovement);
    }
    void OnDisable()
    {
        if (!onPause.RemoveListener(_toggleMovement)) {
            Debug.LogWarning("Listener not found on disable, possible memory leak");
        }
        Character.Events.OnFreeze -= _freeze;
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
        DashTween = agent.transform.DOMove(direction * dashDistance, dashDuration).SetEase(dashEase);
    }
    void OnCollisionEnter(Collision collision)
    {
        bool hitWall = collision.gameObject.CompareTag("Wall");
        if (hitWall) DashTween?.Kill();
    }
    public void StopMovement()
    {
        agent.isStopped = true;
        StopCoroutine(_chaseCoroutine);
    }
    protected override void Init()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        _chaseCoroutine = ChaseCoroutine();
        _toggleMovement = ToggleMovement;
        _freeze = OnFreeze;
    }
    IEnumerator ChaseCoroutine()
    {
        while (true) {
            agent.SetDestination(target.position);
            yield return null;
        }
    }
    void OnFreeze(float duration)
    {
        agent.isStopped = true;
        if (freezeFeedback != null) freezeFeedback.PlayFeedbacks();
        this.Delay(duration, static c => {
            if (c.IsDestroyed() || !c.gameObject.activeInHierarchy) return;
            c.agent.isStopped = false;
        });
    }
}