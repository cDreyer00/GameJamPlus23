using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    [SerializeField] int            damage;
    [SerializeField] float          moveSpeed = 1;
    [SerializeField] float          lifeTime;
    [SerializeField] AnimationCurve curve;

    Transform _transform;
    float     _currentLifeTime;
    Vector3   _startPos;

    public List<string> ignoreList;
    public GenericPool<Projectile> Pool { get; set; }
    public void OnGet(GenericPool<Projectile> pool)
    {
        Pool = pool;
        _currentLifeTime = lifeTime;
        _startPos = transform.position;
    }
    public void OnRelease() {}
    public void OnCreated() {}
    void Awake()
    {
        _currentLifeTime = lifeTime;
        _transform = transform;
    }
    void Update()
    {
        Move(Time.deltaTime);

        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime <= 0) {
            if (Pool != null) {
                Pool.Release(this);
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    void Move(float deltaTime)
    {
        var pos = _transform.position;
        pos += moveSpeed * deltaTime * transform.forward;
        pos.y = _startPos.y * curve.Evaluate(1 - _currentLifeTime / lifeTime);
        _transform.position = pos;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<PlayerController>() != null) return;
        if (Pool != null)
            Pool.Release(this);
        else {
            Destroy(gameObject);
        }
        if (!col.TryGetComponent<MeleeEnemy>(out var enemy)) return;
        enemy.Events.onTakeDamage?.Invoke(damage);
    }
}