using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject model;

    Vector3 mouseDelta = Vector3.zero;
    Vector3 lastMousePos = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            Shoot();

        if (Input.GetMouseButton(0))
        {            
            Rotate();
        }
    }

    void Rotate()
    {
        mouseDelta = Input.mousePosition - lastMousePos;
        model.transform.Rotate(Vector3.forward, mouseDelta.x);
        
        lastMousePos = Input.mousePosition;
    }

    void Shoot()
    {

    }
}
