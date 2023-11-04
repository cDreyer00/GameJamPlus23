using UnityEngine;

public class EffectSign : MonoBehaviour, IPoolable<EffectSign>
{
    [SerializeField] Direction direction;
    [SerializeField] Effect effect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer _circleSr;
    [SerializeField] SpriteRenderer _arrowSr;

    public float Radius => radius;

    public GenericPool<EffectSign> Pool { get; set; }
    public Effect Effect
    {
        get => effect;
        set => effect = value;
    }

    SpriteRenderer _sprite;
    Direction _curCameraDir;
    float _curLifeTime;

    Color ColorByType => effect is FreezeEffect ? Color.cyan : Color.yellow;
    bool CanInteract => _curCameraDir == direction;

    public void Init()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();

        direction = (Direction)Random.Range(0, 4);
        Vector3 lookAt = CameraController.DirectionToVector3(direction) + transform.position;
        transform.LookAt(lookAt);
        transform.localScale = Vector3.one * radius;

        SetColor();

        // set effect
        int randEffect = Random.Range(0, 2);
        effect = randEffect switch { 0 => new FreezeEffect(), 1 => new ConfusionEffect(), _ => null };
        _sprite.color = ColorByType;
        _curLifeTime = lifeTime;

        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;
        OnCamDirectionChanged(CameraController.Instance.Direction);
    }

    void OnValidate()
    {
        transform.localScale = Vector3.one * radius;
    }

    void Update()
    {
        _curLifeTime -= Time.deltaTime;
        if (_curLifeTime <= 0)
        {
            Pool.Release(this);
            return;
        }

        if (CheckInteraction())
        {
            ApplyEffect();
            Pool.Release(this);
        }
    }

    bool CheckInteraction()
    {
        if (!CanInteract) return false;

        float dist = Vector3.Distance(transform.position, GameManager.Instance.Player.Position);
        return dist <= radius;
    }

    void OnCamDirectionChanged(Direction dir)
    {
        _curCameraDir = dir;
        SetColor();
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

    void SetColor()
    {
        Color c = CanInteract ? Color.green : ColorByType;
        _sprite.color = c;
    }

    public void OnRelease()
    {

    }

    public void OnCreated()
    {

    }
}