using Sources.Camera;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Character
{
    [SerializeField] GameObject model;
    [SerializeField] Projectile projPrefab;
    [SerializeField] Transform anchor;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerAim aim;
    [SerializeField] float recoilForce = 3;
    [SerializeField] float shootDelay = 1.3f;
    [SerializeField] float braking = 5f;
    [Space, SerializeField] AudioClip[] shootAudios;
    [SerializeField] AudioClip damageAudio;

    Camera _cam;
    float _curDelay;
    float _baseDrag;
    public float CurDelay => _curDelay;
    public float ShootDelay => shootDelay - (Progress.Instance.upgrades.attackSpeedLevel * 0.02f);

    [SerializeField] CameraShake cameraShake;

    Progress.Upgrades Upgrades => Progress.Instance.upgrades;

    bool useController = true;
    PlayerInputs inputs;
    [SerializeField] Vector2 inputRot;

    bool shooting;

    void Awake()
    {
        inputs = new PlayerInputs();
        inputs.Enable();

        inputs.Gameplay.Shoot.performed += (ctx) => shooting = true;
        inputs.Gameplay.Shoot.canceled += (ctx) => shooting = false;

        inputs.Gameplay.Aim.performed += (ctx) => inputRot = ctx.ReadValue<Vector2>();
        inputs.Gameplay.Aim.canceled += (ctx) => inputRot = Vector2.zero;
    }

    void Start()
    {
        team = "Player";
        _cam = CameraController.Instance.Cam;
        _baseDrag = rb.drag;

        Events.Died += Died;
    }
    void Update()
    {
        if (GameManager.IsGameOver) return;
        if (GameManager.GetGlobalInstance<Spawner>("spawner").IsPaused) return;

        _curDelay += Time.deltaTime;

        if (useController)
        {
            Vector3 lookAtPos = transform.position + new Vector3(inputRot.x, 0, inputRot.y);
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

    void Aim()
    {

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
        Projectile proj = Instantiate(projPrefab, transform.position, anchor.rotation);
        proj.damage = (int)Upgrades.GetModValue(proj.damage, Progress.Upgrades.Type.Damage, 1);
        proj.IgnoreTeam(team);
        Dash(-proj.transform.forward);

        if (shootAudios.Length > 0)
            shootAudios[Random.Range(0, shootAudios.Length)].Play();
    }

    void Dash(Vector3 dir)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * Upgrades.GetModValue(recoilForce, Progress.Upgrades.Type.Recoil, 0.5f), ForceMode.Impulse);
    }

    static void Died(ICharacter character)
    {
        GameManager.Instance.ReloadScene();
    }
}