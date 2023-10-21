using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] GameObject model;
    [SerializeField] Projectile projPrefab;
    [SerializeField] Transform anchor;
    [SerializeField] Rigidbody rb;
    [SerializeField] PlayerAim aim;
    [SerializeField] float dashForce = 3;
    [SerializeField] float shootDelay = 1.3f;
    [SerializeField] float shootDelayDelta = 0.15f;
    [SerializeField] float breakDrag = 5f;

    Camera cam;
    float initShootDelay;
    float curDelay;
    float baseDrag;

    public Vector3 Pos => transform.position;

    private void Awake()
    {
        initShootDelay = shootDelay;
    }

    void Start()
    {
        PowerBar.Instance.onPowerChanged += OnPowerChanged;
        cam = CameraController.Instance.Cam;
        baseDrag = rb.drag;
    }

    void Update()
    {
        curDelay += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            if (curDelay >= shootDelay)
            {
                Shoot();
                curDelay = 0;
            }
        }

        // if (Input.GetMouseButton(0))
        Rotate();

        if (transform.position.y <= -1)
            GameManager.Instance.ReloadScene();

        if (Input.GetKey(KeyCode.Space))
        {
            rb.drag = baseDrag * breakDrag;
        }
        else
        {
            rb.drag = baseDrag;
        }
    }

    void Rotate()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
        Dash(-proj.transform.forward);
    }

    void Dash(Vector3 dir)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * dashForce, ForceMode.Impulse);
    }

    void OnPowerChanged(PowerBarEventArgs args)
    {
        shootDelay = initShootDelay - args.PowerLevel * shootDelayDelta;
    }

    public void TakeDamage(int amount)
    {
        float power = PowerBar.Instance.Power;
        if (power <= 0)
        {
            GameManager.Instance.ReloadScene();
            return;
        }

        PowerBar.Instance.UpdatePower(-amount);
    }
}