using UnityEngine;

namespace Sources.Systems
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField]bool atSameDirection;
        
        Transform cam;

        private void Start()
        {
            if (CompareTag("Enemy"))
            {
                cam = GameObject.Find("Player").transform;
            }
            else
                cam = GameObject.Find("Main Camera").transform;
        }
        void Update()
        {
            if (atSameDirection)
            {
                transform.rotation = cam.transform.rotation;
            }
            else
            {
                Vector3 lookAt = cam.position;
                lookAt.y = transform.position.y;
                transform.LookAt(lookAt);
            }
        }
    }
}
