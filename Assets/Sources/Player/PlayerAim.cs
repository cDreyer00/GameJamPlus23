using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] LineRenderer lr;
    [SerializeField] float distance = 10;

    void Start()
    {
        lr = lr == null ? GetComponent<LineRenderer>() : lr;
    }

    public void SetAim(Vector3 dir)
    {
        dir.Normalize();
        lr.SetPosition(0, transform.localPosition);
        lr.SetPosition(1, transform.localPosition + dir * 10);
    }
}
