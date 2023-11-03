using Sources.cdreyer;
using Sources.cdreyer.GameLogger;
using UnityEngine;

namespace Sources.UI.Gameplay
{
    public class UIGameplay : MonoBehaviour
    {
        [SerializeField] GameObject keysPanel;
        [SerializeField] ButtonBehaviour settingsBtn;

        void Start()
        {
            Helpers.Delay(5f, () => keysPanel.SetActive(false));

            settingsBtn.AddListener(() =>
            {
                GameLogger.Log("Open settings");
            }, InteractionType.ClickUp);
        }
    }
}