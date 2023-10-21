using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            CameraController.Instance.RotateLeft();
        if (Input.GetKeyDown(KeyCode.D))
            CameraController.Instance.RotateRight();

        if (Input.GetKeyDown(KeyCode.R))
            ReloadScene();

    }

    public void ReloadScene()
    {
        LoadingManager.Instance.FadeIn(() =>
                        LoadingManager.Instance.SetLoading(true).LoadScene(SceneType.GAMEPLAY)
                    );
    }
}