using UnityEngine;
using UnityEngine.UI;

namespace Sources.UI.Gameplay
{
    public class UIPlayerReload : MonoBehaviour
    {
        [SerializeField] Image img;

        public void Update()
        {
            float cur = GameManager.PlayerHealthBar.Player.CurDelay;
            float max = GameManager.PlayerHealthBar.Player.ShootDelay;
            img.fillAmount = cur / max;
        }
    }
}
