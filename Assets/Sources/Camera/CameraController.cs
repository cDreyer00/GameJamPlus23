using System;
using System.Collections.Generic;

using UnityEngine;
using DG.Tweening;

public enum Direction
{
    Down,
    Left,
    Up,
    Right,
}
public class CameraController : Singleton<CameraController>
{
    [SerializeField] UnityEngine.Camera cam;
    [SerializeField] Transform camAnchor;
    [SerializeField] RotateMode rotateMode;
    [SerializeField] float rotDuration;
    [SerializeField] Transform wallRaycastAnchor;

    Vector3 curEuler = Vector3.zero;
    HashSet<Transform> walls = new();
    HashSet<Transform> hitWalls = new();

    public Direction Direction => (Direction)(Math.Abs(curEuler.y / 90) % 4);
    public UnityEngine.Camera Cam => cam;

    public event Action<Direction> camDirectionChanged;

    public void RotateLeft()
    {
        curEuler += Vector3.up * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);

        camDirectionChanged?.Invoke(Direction);
    }

    public void RotateRight()
    {
        curEuler += Vector3.down * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);

        camDirectionChanged?.Invoke(Direction);
    }

    readonly RaycastHit[] _hits = new RaycastHit[4];

    private void Update()
    {
        var pos = wallRaycastAnchor.position;
        var dir = wallRaycastAnchor.forward;
        int size = Physics.RaycastNonAlloc(pos, dir, _hits, Mathf.Infinity);

        for (int i = 0; i < size; i++)
        {
            var hit = _hits[i];
            if (hit.collider.transform.childCount == 0) continue;

            var t = hit.collider.transform.GetChild(0);
            if (!hit.collider.CompareTag("Wall")) continue;

            walls.Add(t);
            hitWalls.Add(t);
        }
    }

    private void LateUpdate()
    {
        foreach (var wall in walls)
        {
            wall.gameObject.SetActive(!hitWalls.Contains(wall));
        }

        hitWalls.Clear();
    }

    public void SetRotation(int rotId)
    {
        curEuler = new(0, 90 * rotId, 0);
        camAnchor.DOLocalRotate(curEuler, rotDuration, rotateMode);
    }
}