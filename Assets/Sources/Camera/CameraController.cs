using System;
using System.Collections.Generic;

using UnityEngine;
using DG.Tweening;

public enum Direction
{
    Down,
    Right,
    Up,
    Left,
}
public class CameraController : Singleton<CameraController>
{
    [SerializeField] Camera cam;
    [SerializeField] Transform camAnchor;
    [SerializeField] RotateMode rotateMode;
    [SerializeField] float rotDuration;
    [SerializeField] Transform wallRaycastAnchor;
    [SerializeField] Direction dir;

    Vector3 curEuler = Vector3.zero;
    HashSet<Transform> walls = new();
    HashSet<Transform> hitWalls = new();

    public Direction Direction => (Direction)(CurDirId % 4);
    public Camera Cam => cam;
    int _curDirId = 0;
    int CurDirId
    {
        get => _curDirId;
        set
        {
            _curDirId = value;
            _curDirId = _curDirId < 0 ? 3 : _curDirId;
            _curDirId = _curDirId > 3 ? 0 : _curDirId;
        }
    }

    public event Action<Direction> camDirectionChanged;

    public void RotateLeft()
    {
        CurDirId--;
        curEuler += Vector3.up * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);

        dir = Direction;
        camDirectionChanged?.Invoke(Direction);
    }

    public void RotateRight()
    {
        CurDirId++;
        curEuler += Vector3.down * 90;
        Vector3 endValue = new(0, curEuler.y, 0);
        camAnchor.DOLocalRotate(endValue, rotDuration, rotateMode);

        dir = Direction;
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
            // wall.gameObject.SetActive(!hitWalls.Contains(wall));
            wall.gameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var wallss in wall.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (!hitWalls.Contains(wall))
                {
                    wallss.material.SetFloat("_Opacity", 1f);
                }
                else
                    wallss.material.SetFloat("_Opacity", 0.2f);
            }           
            
        }

        hitWalls.Clear();
    }

    public void SetRotation(int rotId)
    {
        curEuler = new(0, 90 * rotId, 0);
        camAnchor.DOLocalRotate(curEuler, rotDuration, rotateMode);
    }

    public static Vector3 DirectionToVector3(Direction dir) => dir switch
    {
        Direction.Up => Vector3.forward,
        Direction.Down => Vector3.back,
        Direction.Right => Vector3.right,
        Direction.Left => Vector3.left,
        _ => Vector3.forward
    };
}