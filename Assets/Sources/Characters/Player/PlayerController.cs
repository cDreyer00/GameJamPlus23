using Sources.Camera;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

using static Progress;

public class PlayerController : Character
{
    [SerializeField] GameObject model;
    [SerializeField] Projectile projPrefab;
    [SerializeField] Transform anchor;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerAim aim;
    [SerializeField] float damage = 5;
    [SerializeField] float recoilForce = 3;
    [SerializeField] float shootDelay = 1.3f;
    [SerializeField] float braking = 5f;
    [Space, SerializeField] AudioClip[] shootAudios;
    [SerializeField] AudioClip damageAudio;

    [SerializeField] MMFeedbacks shoot;
    [SerializeField] Animator animator;
    
    readonly int _hIsShoot = Animator.StringToHash("isShoot");
    
    Camera                       _cam;
    float                        _curDelay;
    float                        _baseDrag;
    public float CurDelay => _curDelay;
    public float ShootDelay => shootDelay - (Progress.Instance.upgrades.attackSpeedLevel * 0.02f);

    Progress.Upgrades Upgrades => Progress.Instance.upgrades;

    PlayerInputs inputs;
    [SerializeField] Vector2 inputRot;

    bool shooting;

    void OnValidate()
    {
        if (!animator) animator = GetComponentInChildren<Animator>(); 
    }

    void Awake()
    {
        inputs = new PlayerInputs();
        inputs.Gameplay.Enable();

        inputs.Gameplay.Shoot.performed += (ctx) => shooting = true;
        inputs.Gameplay.Shoot.canceled += (ctx) => shooting = false;

        inputs.Gameplay.Aim.performed += (ctx) => inputRot = ctx.ReadValue<Vector2>();
        inputs.Gameplay.Aim.canceled += (ctx) => inputRot = Vector2.zero;

        inputs.Gameplay.RotateCamera.performed += (ctx) =>
        {
            if (ctx.ReadValue<float>() < 0)
                CameraController.Instance.RotateLeft();
            else
                CameraController.Instance.RotateRight();
        };
    }

    void Start()
    {
        team = tag;
        _cam = CameraController.Instance.Cam;
        _baseDrag = rb.drag;

        Events.OnDied += OnDied;
    }

    void OnEnable()
    {
        Progress.Instance.upgrades.OnUpgrade += OnUpgrade;
    }

    void OnDisable()
    {
        Progress.Instance.upgrades.OnUpgrade -= OnUpgrade;
    }

    void Update()
    {
        if (GameManager.IsGameOver) return;
        if (GameManager.GetGlobalInstance<Spawner>("spawner").IsPaused) return;

        _curDelay += Time.deltaTime;

        if (GameManager.Instance.useController)
        {
            // Get the camera's rotation
            Quaternion cameraRotation = CameraController.Instance.Cam.transform.rotation;

            // Transform the input by the camera's rotation
            Vector3 transformedInput = cameraRotation * new Vector3(inputRot.x, 0, inputRot.y);

            // Apply the transformed input to the player's rotation
            Vector3 lookAtPos = transform.position + transformedInput;
            lookAtPos.y = anchor.position.y;
            anchor.LookAt(lookAtPos, Vector3.up);
            aim.SetAim(anchor.forward);
            
            if (shooting)
            {
                if (_curDelay >= ShootDelay)
                {
                    Shoot();
                    _curDelay = 0;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                if (_curDelay >= ShootDelay)
                {
                    Shoot();
                    _curDelay = 0;
                }
            }

            Rotate();

            if (Input.GetKey(KeyCode.Space))
            {
                // rb.drag = _baseDrag * braking;
                rb.drag = _baseDrag * Upgrades.GetModValue(braking, Progress.Upgrades.Type.Braking);
            }
            else
            {
                rb.drag = _baseDrag;
            }
        }


        if (transform.position.y <= -1)
        {
            GameManager.Instance.ReloadScene();
        }
    }

    Vector3 GetPosBasedOnCameraView(Vector3 pos)
    {
        var cam = CameraController.Instance.Cam;
        var camPos = cam.transform.position;
        var camDir = cam.transform.forward;
        var dir = pos - camPos;
        var dot = Vector3.Dot(dir, camDir);
        var proj = camPos + camDir * dot;
        return proj;
    }


    void Rotate()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Vector3 lookAtPos = hit.point;
            lookAtPos.y = anchor.position.y;
            anchor.LookAt(lookAtPos, Vector3.up);
            aim.SetAim(anchor.forward);
        }
    }

    void Shoot()
    {
        animator.SetTrigger(_hIsShoot);
        
        Projectile proj = Instantiate(projPrefab, transform.position, anchor.rotation);
        proj.Damage = (int)Upgrades.GetModValue(damage, Progress.Upgrades.Type.Damage, 1.5f);
        proj.IgnoreTeam(team);
        Dash(-proj.transform.forward);

        if (shoot != null) shoot.PlayFeedbacks();

        if (shootAudios.Length > 0)
            shootAudios.GetRandom().Play();
    }

    void Dash(Vector3 dir)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * Upgrades.GetModValue(recoilForce, Progress.Upgrades.Type.Recoil, 0.5f), ForceMode.Impulse);
    }

    static void OnDied(ICharacter character)
    {
        GameManager.Instance.ReloadScene();
    }

    void OnUpgrade(Upgrades.Type type, int level) => OnUpgrade(type);
    void OnUpgrade(Upgrades.Type type)
    {
        var hm = GetModule<HealthModule>();

        if (type == Upgrades.Type.Health)
            hm.MaxHealth = Upgrades.GetModValue(hm.BaseHealth, type);

        hm.Health = hm.MaxHealth;
    }
}