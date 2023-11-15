using UnityEngine;
using UnityEngine.UI;

namespace Sources.UI.Gameplay
{
    public class UIPlayerReload : MonoBehaviour
    {
        [SerializeField] Image img;

        public void Update()
        {
            PlayerController pc = (PlayerController)GameManager.Instance.Player;
            float cur = pc.CurDelay;
            float max = pc.ShootDelay;
            img.fillAmount = cur / max;
        }
    }
}
