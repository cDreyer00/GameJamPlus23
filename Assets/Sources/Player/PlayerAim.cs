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
        var myPos = transform.localPosition;
        Physics.Raycast(myPos, dir, out var hit, distance, LayerMask.GetMask("Enemy"));

        var hitPos = hit.point;
        float dist = Vector3.Distance(myPos, hitPos);
        dir.Normalize();
        lr.SetPosition(0, myPos);
        lr.SetPosition(1, myPos + dir * dist);
    }
}