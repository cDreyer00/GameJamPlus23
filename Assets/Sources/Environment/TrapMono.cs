using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrapMono : MonoBehaviour
{
    public Direction TrapDirection;

    SpriteRenderer sprite;

    public float radius = 1f;

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.localScale = Vector3.one * radius;
    }

    private void Update()
    {
        if (CameraController.Instance.Direction == TrapDirection)
        {
            print("Trap is active");
            sprite.color = Color.green;
        }
        else
        {
            sprite.color = Color.gray;
        }

        if (Vector3.Distance(GameManager.Instance.Player.Pos, transform.position) < radius)
        {
            OnTrapTriggered?.Invoke(this);
        }
    }

    public Action<TrapMono> OnTrapTriggered;
}