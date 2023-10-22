using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat : MonoBehaviour
{
    Transform cam;

    private void Start()
    {
        cam = GameObject.Find("Main Camera").transform;
    }
    void Update()
    {
        Vector3 lookAt = cam.position;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
    }
}
