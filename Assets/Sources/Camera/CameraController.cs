using System;
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
    [SerializeField] Transform wallRaycastAnchor;

    Vector3 curEuler = Vector3.zero;
    HashSet<MeshRenderer> walls = new();

    public Camera Cam => cam;

    public void RotateLeft()
    {
        curEuler += Vector3.up * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);
    }

    public void RotateRight()
    {
        curEuler += Vector3.down * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(wallRaycastAnchor.position, wallRaycastAnchor.forward, out hit, Mathf.Infinity))
        {
            var mesh = hit.collider.GetComponent<MeshRenderer>();
            if (hit.collider.CompareTag("Wall"))
            {
                walls.Add(mesh);
                mesh.enabled = false;
            }
            foreach (var meshRenderer in walls)
            {
                if (meshRenderer != mesh)
                    meshRenderer.enabled = true;
            }
        }
    }

    public void SetRotation(int rotId)
    {
        curEuler = new(0, 90 * rotId, 0);
        camAnchor.DOLocalRotate(curEuler, rotDuration, rotateMode);
    }
}