using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] GameObject model;
    [SerializeField] Projectile projPrefab;
    [SerializeField] Transform anchor;
    [SerializeField] float dashForce = 3;
    [SerializeField] float shootDelay = 1.3f;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;

    float curDelay;

    private void Awake()
    {
        cam = cam != null ? cam : Camera.main;
    }

    void Update()
    {
        curDelay += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            if (curDelay >= shootDelay)
            {
                Shoot();
                curDelay = 0;
            }
        }

        if (Input.GetMouseButton(0))
            Rotate();
    }

    void Rotate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Vector3 lookAtPos = hit.point;
            lookAtPos.y = anchor.position.y;
            anchor.LookAt(lookAtPos, Vector3.up);
        }
    }

    void Shoot()
    {
        Projectile proj = Instantiate(projPrefab, transform.position, anchor.rotation);
        Dash(-proj.transform.forward);
    }

    void Dash(Vector3 dir)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
    }

    public void TakeDamage(int amount)
    {
        throw new NotImplementedException();
    }
}