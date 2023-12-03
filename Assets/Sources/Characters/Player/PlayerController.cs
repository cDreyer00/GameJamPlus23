using Sources.Camera;
using UnityEngine;

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

    void Start()
    {
        team = "Player";
        _cam = CameraController.Instance.Cam;
        _baseDrag = rb.drag;

        Events.OnDied += OnDied;
    }
    void Update()
    {
        if (GameManager.IsGameOver) return;

        _curDelay += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (_curDelay >= ShootDelay)
            {
                Shoot();
                _curDelay = 0;
            }
        }

        Rotate();

        if (transform.position.y <= -1)
        {
            GameManager.Instance.ReloadScene();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            // rb.drag = _baseDrag * braking;
            rb.drag = _baseDrag * Upgrades.GetModValue(braking, Progress.Upgrades.Type.Barking);
        }
        else
        {
            rb.drag = _baseDrag;
        }
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

    static void OnDied(ICharacter character)
    {
        GameManager.Instance.ReloadScene();
    }
}