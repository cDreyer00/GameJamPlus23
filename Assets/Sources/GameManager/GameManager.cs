using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    bool RotateLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
    bool RotateRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.E);

    void Update()
    {
        if (RotateLeft)
            CameraController.Instance.RotateLeft();
        if (RotateRight)
            CameraController.Instance.RotateRight();

        if (Input.GetKeyDown(KeyCode.R))
            ReloadScene();
    }

    public void ReloadScene()
    {
        LoadingManager.Instance.FadeIn(() =>
        {
            LoadingManager.Instance.SetLoading(true).LoadScene(SceneType.GAMEPLAY);
        });
    }
}