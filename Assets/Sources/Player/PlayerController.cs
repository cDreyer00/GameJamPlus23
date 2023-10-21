using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject model;
    [SerializeField] Projectile projPrefab;
    [SerializeField] Transform rotTf;
    [SerializeField] float dashForce = 3;
    [SerializeField] float shootDelay = 1.3f;
    [SerializeField] Rigidbody rb;
    [SerializeField] Camera cam;

    Vector3 mouseDelta = Vector3.zero;
    Vector3 lastMousePos = Vector3.zero;
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

            lastMousePos = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Rotate();
        }
    }

    void Rotate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
            transform.LookAt(hit.point, Vector3.up);
    }

    void Shoot()
    {
        Projectile proj = Instantiate(projPrefab, transform.position, rotTf.rotation);
        Dash(-proj.transform.forward);
    }

    void Dash(Vector3 dir)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
    }
}