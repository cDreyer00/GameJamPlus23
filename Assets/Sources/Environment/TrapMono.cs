using System;
using System.Collections;
using System.Collections.Generic;
using Sources.Environment;

using UnityEngine;
using CDreyer;
using Sources.Enemy;

public class TrapMono : MonoBehaviour
{
    [SerializeField] Direction direction;
    [SerializeField] Effect effect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    public float Radius => radius;

    public event Action<TrapMono> onTrapDisabled;
    SpriteRenderer sprite;

    public void Init()
    {
        direction = (Direction)UnityEngine.Random.Range(0, 4);
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.localScale = Vector3.one * radius;

        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;
    }

    void OnCamDirectionChanged(Direction dir)
    {
        if (dir == direction)
        {
            sprite.color = Color.green;
            if (Vector3.Distance(GameManager.Instance.Player.Pos, transform.position) < radius)
            {
                Trigger();
            }
        }
        else
        {
            sprite.color = Color.gray;
        }
    }

    void Trigger()
    {
        foreach (var e in EnemySpawner.Instance.Instances)
            EffectManager.ApplyEffect(e, effect);

        Disable();
    }

    void Disable()
    {
        onTrapDisabled?.Invoke(this);
        Destroy(gameObject);
    }
}