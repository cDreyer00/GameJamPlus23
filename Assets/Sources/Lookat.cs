using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookat : MonoBehaviour
{
    [SerializeField] Transform cam;
    void Update()
    {
        transform.LookAt(cam);
    }
}
