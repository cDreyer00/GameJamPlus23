using System;
using System.Collections;
using System.Collections.Generic;
using Sources.Environment;

using UnityEngine;
using CDreyer;
using Sources.Enemy;
using Unity.VisualScripting;
using UnityEditor;

public class TrapMono : MonoBehaviour
{
    [SerializeField] Direction direction;
    [SerializeField] Effect effect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    public float Radius => radius;
    public Effect Effect { get => effect; set => effect = value; }

    public event Action<TrapMono> onTrapDisabled;
    SpriteRenderer sprite;
    Direction curCameraDir;

    public void Init()
    {
        direction = (Direction)UnityEngine.Random.Range(0, 4);
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.localScale = Vector3.one * radius;

        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;

        OnCamDirectionChanged(CameraController.Instance.Direction);

        Helpers.ActionCallback(Disable, lifeTime);
    }

    void OnCamDirectionChanged(Direction dir)
    {
        curCameraDir = dir;
        // if (dir == direction)
        // {
        //     sprite.color = Color.green;
        // }
        // else
        // {
        //     sprite.color = Color.gray;
        // }
    }

    void Update()
    {
        // if (curCameraDir != direction) return;

        if (Vector3.Distance(GameManager.Instance.Player.Pos, transform.position) < radius)
            Trigger();
    }

    void Trigger()
    {
        foreach (var e in EnemySpawner.Instance.Instances)
            EffectManager.ApplyEffect(e, effect, effectDuration);

        Disable();
    }

    void Disable()
    {
        if (this.IsDestroyed()) return;

        onTrapDisabled?.Invoke(this);
        Destroy(gameObject);

        CameraController.Instance.camDirectionChanged -= OnCamDirectionChanged;
    }
}