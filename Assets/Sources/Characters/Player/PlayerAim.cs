using UnityEngine;

namespace Sources.Characters.Player
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField] LineRenderer lr;
        [SerializeField] float        distance = 10;

        void Start()
        {
            lr = lr == null ? GetComponent<LineRenderer>() : lr;
        }

        public void SetAim(Vector3 dir)
        {
            float dist = distance;
            var   pos  = transform.localPosition;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
            {
                dist = Vector3.Distance(transform.position, hit.point);
            }

            dir.Normalize();
            lr.SetPosition(0, pos);
            lr.SetPosition(1, pos + dir * dist);
        }
    }
}