using UnityEngine;

public class EffectSign : MonoBehaviour, IPoolable<EffectSign>
{
    [SerializeField] Direction direction;
    [SerializeField] Effect effect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    public float Radius => radius;

    public GenericPool<EffectSign> Pool { get; set; }
    public Effect Effect
    {
        get => effect;
        set => effect = value;
    }

    SpriteRenderer _sprite;
    Direction _curCameraDir;

    Color BaseColor => effect is FreezeEffect ? Color.cyan : Color.yellow;

    public void Init()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();

        direction = (Direction)Random.Range(0, 4);
        Vector3 lookAt = CameraController.DirectionToVector3(direction) + transform.position;
        transform.LookAt(lookAt);
        transform.localScale = Vector3.one * radius;

        // set effect
        int randEffect = Random.Range(0, 2);
        effect = randEffect switch { 0 => new FreezeEffect(), 1 => new ConfusionEffect() };
        _sprite.color = BaseColor;

        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;
        OnCamDirectionChanged(CameraController.Instance.Direction);
    }

    void OnValidate()
    {
        transform.localScale = Vector3.one * radius;
    }

    void Update()
    {
        Color c = _curCameraDir == direction ? Color.green : BaseColor;
        _sprite.color = c;

        float dist = Vector3.Distance(transform.position, GameManager.Instance.Player.Position);
        if (dist <= radius)
        {
            ApplyEffect();
        }
    }

    void OnCamDirectionChanged(Direction dir)
    {
        _curCameraDir = dir;
    }

    void ApplyEffect()
    {
        var enemies = SpawnerSrevice.MeleeEnemySpawner.GetAllEnemies();
        foreach (var e in enemies)
            effect.ApplyEffect(e);
    }

    public void OnGet(GenericPool<EffectSign> pool)
    {
        this.Pool = pool;
    }

    public void OnRelease()
    {

    }

    public void OnCreated()
    {

    }
}