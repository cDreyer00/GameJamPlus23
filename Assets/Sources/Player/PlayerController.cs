using UnityEngine;

namespace Sources.Player
{
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
        [Space][SerializeField] AudioClip[] shootAudios;
        [SerializeField] AudioClip damageAudio;

        Camera cam;
        float initShootDelay;
        float curDelay;
        float baseDrag;

        public Vector3 Pos => transform.position;

        public float CurDelay => curDelay;
        public float ShootDelay => shootDelay;

        [SerializeField] FeedbackDamage feed;
        [SerializeField] CameraShake came;
        private void Awake()
        {
            initShootDelay = shootDelay;
        }

        void Start()
        {
            PowerBar.Instance.onPowerChanged += OnPowerChanged;
            cam = CameraController.Instance.Cam;
            baseDrag = rb.drag;

            GameManager.Instance.RegisterPlayer(this);
        }

        void Update()
        {
            if (GameManager.IsGameOver)
                return;

            curDelay += Time.deltaTime;
            if (Input.GetMouseButton(0))
            {
                if (curDelay >= shootDelay)
                {
                    Shoot();
                    curDelay = 0;
                }
            }

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

            if (shootAudios.Length > 0)
                shootAudios[Random.Range(0, shootAudios.Length)].Play();
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
            if (GameManager.IsGameOver)
                return;
            feed.StartCoroutine("DamageColor");
            if (came != null) came.ShakeCamera();

            if (damageAudio != null)
                damageAudio.Play();

            float power = PowerBar.Instance.Power;
            float maxPower = PowerBar.Instance.MaxPower;
            if (power >= maxPower)
            {
                GameManager.Instance.ReloadScene();
                return;
            }

            PowerBar.Instance.UpdatePower(-amount);
        }
    }
}