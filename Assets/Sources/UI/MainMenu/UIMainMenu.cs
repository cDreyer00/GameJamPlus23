using Sources.cdreyer;
using UnityEngine;

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