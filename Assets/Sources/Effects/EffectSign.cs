using UnityEngine;

public class EffectSign : MonoBehaviour, IPoolable<EffectSign>
{
    [SerializeField] Direction direction;
    [SerializeField] Effect effect;
    [SerializeField] float radius = 1f;
    [SerializeField] float effectDuration;
    [SerializeField] float lifeTime;

    public float Radius => radius;

    GenericPool<EffectSign> _pool;
    public GenericPool<EffectSign> Pool
    {
        get => _pool; set
        {
            _pool = value;
            Debug.Log($"pool setted for sign, is null -> {_pool == null}");
        }
    }
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

    void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
        Init();
    }

    public void Init()
    {
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

        OnCamDirectionChanged(CameraController.Instance.Direction);
    }

    void OnValidate()
    {
        transform.localScale = Vector3.one * radius;
    }

    void OnEnable()
    {
        CameraController.Instance.camDirectionChanged += OnCamDirectionChanged;
    }

    void OnDisable()
    {
        CameraController.Instance.camDirectionChanged -= OnCamDirectionChanged;
    }

    void Update()
    {
        _curLifeTime -= Time.deltaTime;
        if (_curLifeTime <= 0)
        {
            // Pool.Release(this);
            Destroy(gameObject);
            return;
        }

        if (CheckInteraction())
        {
            ApplyEffect();
            // Pool.Release(this);
            Destroy(gameObject);
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
        //TODO: apply effect to all enemies in radius
        // var enemies = SpawnerSrevice.MeleeEnemySpawner.GetAllEnemies();
        // foreach (var e in enemies)
        //     effect.ApplyEffect(e);
    }

    public void OnGet()
    {

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