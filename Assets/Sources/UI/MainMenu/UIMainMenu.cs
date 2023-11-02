using Sources.cdreyer;
using Sources.cdreyer.loading_system;
using UnityEngine;

namespace Sources.UI.MainMenu
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] ButtonBehaviour startBtn;

        void Start()
        {
            startBtn.AddListener(OnStartButtonClick, InteractionType.ClickUp);
        }

        void OnStartButtonClick()
        {
            LoadingManager.Instance.FadeIn(() =>
                LoadingManager.Instance.LoadScene(SceneType.GAMEPLAY)
            );
        }
    }
}
