using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] MeshRenderer groundMr;
    
    public IPlayer Player { get; private set; }
    public void RegisterPlayer(IPlayer p) => Player = p;

    bool RotateLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
    bool RotateRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.E);

    public Bounds GameBounds { get => groundMr == null ? new(Vector3.zero, Vector3.zero) : groundMr.bounds; }

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

        SoundManager.Instance.Stop();
    }

    int GetCamId()
    {
        Vector3 magnitudeVector = Player.Pos;
        Vector3 normalizedVector = magnitudeVector.normalized;

        float maxAbsValue = Mathf.Max(Mathf.Abs(normalizedVector.x), Mathf.Abs(normalizedVector.y), Mathf.Abs(normalizedVector.z));

        Vector3 closestDirectionVector;

        if (Mathf.Approximately(maxAbsValue, Mathf.Abs(normalizedVector.x)))
        {
            closestDirectionVector = new Vector3(Mathf.Sign(normalizedVector.x), 0, 0);
        }
        else if (Mathf.Approximately(maxAbsValue, Mathf.Abs(normalizedVector.y)))
        {
            closestDirectionVector = new Vector3(0, Mathf.Sign(normalizedVector.y), 0);
        }
        else
        {
            closestDirectionVector = new Vector3(0, 0, Mathf.Sign(normalizedVector.z));
        }

        if (closestDirectionVector.x == -1)
            return 1;
        if (closestDirectionVector.z == 1)
            return 2;
        if (closestDirectionVector.x == 1)
            return 3;
        if (closestDirectionVector.z == -1)
            return 0;

        return 0;
    }
}