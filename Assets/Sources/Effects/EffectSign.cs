using UnityEngine;

public class EffectSign : MonoBehaviour, IPoolable<EffectSign>
{
    [SerializeField] EffectType   effectType;
    [SerializeField] float        radius = 1f;
    [SerializeField] float        effectDuration;
    [SerializeField] float        lifeTime;
    [SerializeField] Material     baseMat, interactableMat;
    [SerializeField] MeshRenderer botao;

    Effect                  effect;
    Direction               direction;
    GenericPool<EffectSign> _pool;
    //SpriteRenderer _sprite;
    Direction _curCameraDir;
    float     _curLifeTime;

    public float Radius => radius;

    public GenericPool<EffectSign> Pool
    {
        get => _pool;
        set { _pool = value; }
    }

    public Effect Effect
    {
        get => effect;
        set => effect = value;
    }

    //Color ColorByType => effect is FreezeEffect ? Color.cyan : Color.red;
    bool CanInteract => _curCameraDir == direction;

    void Start()
    {
        //_sprite = GetComponentInChildren<SpriteRenderer>();
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
        effect = effectType switch
        {
            EffectType.Freeze    => new FreezeEffect(),
            EffectType.Confusion => new ConfusionEffect(),
            EffectType.Damage    => new DamageEffect(),
            _                    => null
        };
        effect.duration = effectDuration;
        effect.damage = effectDuration;

        //_sprite.color = ColorByType;
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
        if (_curLifeTime <= 0) {
            // Pool.Release(this);
            Destroy(gameObject);
            return;
        }

        if (CheckInteraction()) {
            ApplyEffect();
            // Pool.Release(this);
            Destroy(gameObject);
        }
    }

    bool CheckInteraction()
    {
        if (!CanInteract) return false;

        if (!GameManager.Instance.Player) return false;

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
        var spawner = GlobalInstancesBehaviour.GlobalInstances.GetInstance<Spawner>("spawner"); /* GameManager.Instance.Spawner */
        ;
        var enemies = spawner.GetInstancesByTag<Character>("Enemy");
        foreach (var e in enemies)
            effect.ApplyEffect(e);
    }

    public void OnGet() {}

    void SetColor()
    {
        Material c = CanInteract ? interactableMat : baseMat;
        //_sprite.color = c;
        var mats = botao.materials;
        mats[1] = c;
        botao.materials = mats;
    }

    public void OnRelease() {}

    public void OnCreated() {}
}

public enum EffectType { Freeze, Confusion, Damage }