using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject model;

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
    }

    void Rotate()
    {
        if (lastMousePos == Vector3.zero)
            lastMousePos = Input.mousePosition;
            
        mouseDelta = Input.mousePosition - lastMousePos;
        GameLogger.Log(mouseDelta.x);
        model.transform.eulerAngles += Vector3.up * mouseDelta.x;

        lastMousePos = Input.mousePosition;
    }

    void Shoot()
    {

    }
}
