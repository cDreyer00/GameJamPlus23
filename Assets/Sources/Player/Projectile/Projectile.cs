using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1;

    void Start()
    {
        Helpers.ActionCallback(() => Destroy(gameObject), 4);
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}