using Sources.Camera;
using Sources.SoundManager;
using Sources.Systems;
using UnityEngine;

namespace Sources.Characters.Player
{
    public class PlayerController : MonoBehaviour, IPlayer
    {
        [SerializeField]        GameObject            model;
        [SerializeField]        Projectile.Projectile projPrefab;
        [SerializeField]        Transform             anchor;
        [SerializeField]        Rigidbody             rb;
        [SerializeField]        PlayerAim             aim;
        [SerializeField]        float                 dashForce  = 3;
        [SerializeField]        float                 shootDelay = 1.3f;
        [SerializeField]        float                 breakDrag  = 5f;
        [Space, SerializeField] AudioClip[]           shootAudios;
        [SerializeField]        AudioClip             damageAudio;

        UnityEngine.Camera _cam;
        float  _curDelay;
        float  _baseDrag;

        public Vector3 Position => transform.position;

        public float CurDelay => _curDelay;
        public float ShootDelay => shootDelay;

        [SerializeField] FeedbackDamage feed;
        [SerializeField] CameraShake    cameraShake;
        public CameraShake Came => cameraShake;

        void Start()
        {
            _cam = CameraController.Instance.Cam;
            _baseDrag = rb.drag;

            GameManager.GameManager.Instance.RegisterPlayer(this);
        }

        void Update()
        {
            if (GameManager.GameManager.IsGameOver) return;

            _curDelay += Time.deltaTime;
            if (Input.GetMouseButton(0)) {
                if (_curDelay >= shootDelay) {
                    Shoot();
                    _curDelay = 0;
                }
            }

            Rotate();

            if (transform.position.y <= -1) {
                GameManager.GameManager.Instance.ReloadScene();
            }
            if (Input.GetKey(KeyCode.Space)) {
                rb.drag = _baseDrag * breakDrag;
            }
            else {
                rb.drag = _baseDrag;
            }
        }

        void Rotate()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
                Vector3 lookAtPos = hit.point;
                lookAtPos.y = anchor.position.y;
                anchor.LookAt(lookAtPos, Vector3.up);
                aim.SetAim(anchor.forward);
            }
        }

        void Shoot()
        {
            Projectile.Projectile proj = Instantiate(projPrefab, transform.position, anchor.rotation);
            Dash(-proj.transform.forward);

            if (shootAudios.Length > 0)
                shootAudios[Random.Range(0, shootAudios.Length)].Play();
        }

        void Dash(Vector3 dir)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * dashForce, ForceMode.Impulse);
        }

        public void TakeDamage(int amount)
        {
            if (GameManager.GameManager.IsGameOver) return;

            feed.StartCoroutine(nameof(FeedbackDamage.DamageColor));
            
            if (cameraShake) cameraShake.ShakeCamera();
            if (damageAudio) damageAudio.Play();

            float hp = HealthBar.Instance.HealthPoints;
            if (hp > 0) {
                HealthBar.Instance.Damage(amount);
            }
            else {
                GameManager.GameManager.Instance.ReloadScene();
            }
        }
    }
}