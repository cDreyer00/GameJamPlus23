using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float lifeTime;

    void Start()
    {
        Helpers.ActionCallback(() => Destroy(gameObject), lifeTime);
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider col)
    {
        GameLogger.Log("projectile collided");
        if (!col.TryGetComponent<IEnemy>(out var enemy)) return;

        enemy.TakeDamage(damage);
    }
}