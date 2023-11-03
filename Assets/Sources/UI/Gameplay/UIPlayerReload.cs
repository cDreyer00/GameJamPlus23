using UnityEngine;
using UnityEngine.UI;

namespace Sources.UI.Gameplay
{
    public class UIPlayerReload : MonoBehaviour
    {
        [SerializeField] Image img;

        public void Update()
        {
            float cur = GameManager.GameManager.Instance.Player.CurDelay;
            float max = GameManager.GameManager.Instance.Player.ShootDelay;
            img.fillAmount = cur / max;
        }
    }
}
