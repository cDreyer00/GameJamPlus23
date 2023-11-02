using UnityEngine;
using UnityEngine.UI;

namespace Sources.UI.Gameplay
{
    public class UIPlayerReload : MonoBehaviour
    {
        [SerializeField] Image img;

        public void Update()
        {
            float cur = GameManager.Instance.Player.CurDelay;
            float max = GameManager.Instance.Player.ShootDelay;
            img.fillAmount = cur / max;
        }
    }
}
