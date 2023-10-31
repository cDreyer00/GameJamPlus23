using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] ButtonBehaviour startBtn;

    void Start()
    {
        startBtn.AddListener(OnStartButtonClick, InteractionType.ClickUp);
    }

    void OnStartButtonClick()
    {
        LoadingManager.PlayerHealthBar.FadeIn(() =>
            LoadingManager.PlayerHealthBar.LoadScene(SceneType.GAMEPLAY)
        );
    }
}
