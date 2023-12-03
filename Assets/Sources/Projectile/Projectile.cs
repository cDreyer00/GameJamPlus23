using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    [SerializeField] public int damage;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float lifeTime;
    [SerializeField] AnimationCurve yAxisTrajectory;

    public Vector3 target;

    Transform _transform;
    float _currentLifeTime;
    Vector3 _anchor;


    public List<string> ignoreList;
    public GenericPool<Projectile> Pool { get; set; }
    public void OnGet()
    {
        Init();
    }
    public void OnRelease()
    {
        Init();
    }
    public void OnCreated()
    {
        Init();
    }
    void Awake()
    {
        Init();
    }
    void Init()
    {
        _currentLifeTime = lifeTime;
        _transform = transform;
        _anchor = _transform.position;
    }
    void Update()
    {
        Move(moveSpeed * Time.deltaTime);

        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0)
        {
            if (Pool != null) Pool.Release(this);
            else Destroy(gameObject);
        }
    }
    public void IgnoreTeam(string team)
    {
        ignoreList ??= new List<string>();
        if (!ignoreList.Contains(team))
            ignoreList.Add(team);
    }
    void Move(float step)
    {
        var position = _transform.position;
        var forward = _transform.forward;

        var maxSqrDistance = Vector3.SqrMagnitude(_anchor - target);
        var sqrDistance = Vector3.SqrMagnitude(target - position);
        // var magnitude01 = ClampedPrimitiveExtensions.MapRangeTo01(sqrDistance, 0, maxSqrDistance);

        //print(magnitude01);

        // float eval = yAxisTrajectory.Evaluate(magnitude01);
        position.x += step * forward.x;
        position.z += step * forward.z;
        // position.y = _anchor.y * (1 + eval);
        _transform.position = position;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<Character>(out var character))
        {
            if (ignoreList.Contains(character.team)) {
                return;
            }

            character.Events.OnTakeDamage(damage);
        }
        if (Pool != null) Pool.Release(this);
        else Destroy(gameObject);
    }
}