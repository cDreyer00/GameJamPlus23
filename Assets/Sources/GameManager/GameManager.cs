using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] MeshRenderer groundMr;
    [SerializeField] Canvas endGameCanvas;

    public Character Player { get; private set; }

    public static bool IsGameOver { get; private set; }

    public void RegisterPlayer(Character player) => Player = player;

    bool RotateLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
    bool RotateRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.E);

    bool fading;

    public Bounds GameBounds
    {
        get => groundMr == null ? new(Vector3.zero, Vector3.zero) : groundMr.bounds;
    }

    float _initTime;
    public float GameElapsedTime => Time.time - _initTime;

    void Start()
    {
        _initTime = Time.time;
    }

    void Update()
    {
        if (RotateLeft)
            CameraController.Instance.RotateLeft();
        if (RotateRight)
            CameraController.Instance.RotateRight();

        if (Input.GetKeyDown(KeyCode.R)) ReloadScene();
    }

    public void ReloadScene()
    {
        if (fading) return;

        fading = true;
        LoadingManager.Instance.FadeIn(() =>
        {
            LoadingManager.Instance.SetLoading(true).LoadScene(SceneType.GAMEPLAY);
            IsGameOver = false;
            fading = false;
        });
        SoundManager.Instance.Stop();
    }

    public void ShowEndGame()
    {
        if (fading) return;
        IsGameOver = true;
        fading = true;
        LoadingManager.Instance.FadeIn(() => endGameCanvas.gameObject.SetActive(true));
        SoundManager.Instance.Stop();
        fading = false;

    }

    int GetCamId()
    {
        Vector3 magnitudeVector = Player.Position;
        Vector3 normalizedVector = magnitudeVector.normalized;

        float maxAbsValue = Mathf.Max(Mathf.Abs(normalizedVector.x), Mathf.Abs(normalizedVector.y),
            Mathf.Abs(normalizedVector.z));

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