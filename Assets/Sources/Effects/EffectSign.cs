using Sources.Camera;
using UnityEngine;
using UnityEngine.Serialization;

public class EffectSign : MonoBehaviour
{
    [SerializeField] Direction direction;
    [FormerlySerializedAs("effect")][SerializeField] BaseEffect baseEffect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    public float Radius => radius;
    public BaseEffect BaseEffect
    {
        get => baseEffect;
        set => baseEffect = value;
    }

    SpriteRenderer _sprite;
    Direction _curCameraDir;

    public void Init()
    {
        direction = (Direction)UnityEngine.Random.Range(0, 4);
        _sprite = GetComponentInChildren<SpriteRenderer>();
        transform.localScale = Vector3.one * radius;

        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;

        OnCamDirectionChanged(CameraController.Instance.Direction);
    }

    void OnCamDirectionChanged(Direction dir)
    {
        _curCameraDir = dir;
    }

    void Update()
    {
        Color c = _curCameraDir == direction ? Color.green : Color.white;
        _sprite.color = c;
    }
}