using System;
using UnityEngine;

public class EffectSign : MonoBehaviour
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
    }

    void OnCamDirectionChanged(Direction dir)
    {
        curCameraDir = dir;
    }

    void Update()
    {
        Color c = curCameraDir == direction ? Color.green : Color.white;
        sprite.color = c;
    }
}