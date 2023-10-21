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
    [SerializeField] Rigidbody rb;

    Vector3 mouseDelta = Vector3.zero;
    Vector3 lastMousePos = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Shoot();
            lastMousePos = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
            Rotate();

        // Color color = Color.red;
        // Debug.DrawLine(transform.position, Forward, color);
    }

    void Rotate()
    {
        if (lastMousePos == Vector3.zero)
            lastMousePos = Input.mousePosition;

        mouseDelta = Input.mousePosition - lastMousePos;
        rotTf.transform.eulerAngles += Vector3.up * mouseDelta.x;

        lastMousePos = Input.mousePosition;
    }


    // void Rotate()
    // {
    //     Vector3 mousePostToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     rotTf.transform.LookAt(mousePostToWorld, Vector3.forward);
    // }

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
