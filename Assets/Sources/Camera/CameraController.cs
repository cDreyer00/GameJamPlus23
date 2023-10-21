using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;
using DG.Tweening;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] Camera cam;
    [SerializeField] Transform camAnchor;
    [SerializeField] RotateMode rotateMode;
    [SerializeField] float rotDuration;

    Vector3 curEuler = Vector3.zero;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            RotateLeft();
        if (Input.GetKeyDown(KeyCode.D))
            RotateRight();
    }

    [ContextMenu("Rotate left")]
    public void RotateLeft()
    {
        curEuler += Vector3.up * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);
    }

    [ContextMenu("Rotate Right")]
    public void RotateRight()
    {
        curEuler += Vector3.down * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);
    }
}
