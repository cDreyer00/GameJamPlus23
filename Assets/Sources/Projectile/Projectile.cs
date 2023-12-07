using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float lifeTime;

    public int Damage { get; set; }
    public Vector3 Target { get; set; }

    protected Transform _transform;
    float _currentLifeTime;
    protected Vector3 _anchor;

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
    protected virtual void Move(float step)
    {
        // go forward
        transform.Translate(Vector3.forward * step);                
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.TryGetComponent<Character>(out var character))
        {
            if (ignoreList.Contains(character.team))
            {
                return;
            }

            character.Events.TakeDamage(Damage);
        }
        if (Pool != null) Pool.Release(this);
        else Destroy(gameObject);
    }
}