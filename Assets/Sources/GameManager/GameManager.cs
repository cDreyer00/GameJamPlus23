using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] MeshRenderer groundMr;
    [SerializeField] Canvas       endGameCanvas;
    [SerializeField] Spawner      spawner;

    public bool useController;

    Scene _currentScene;
    float _initTime;
    bool  fading;

    public Bounds GameBounds        => groundMr == null ? new(Vector3.zero, Vector3.zero) : groundMr.bounds;
    public float GameElapsedTime    => Time.time - _initTime;
    public Spawner Spawner          => spawner;

    bool RotateLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
    bool RotateRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.E);
    bool ChangeInputs => Input.GetKeyDown(KeyCode.Space);

    [CanBeNull] Character _player;
    public Character Player => _player = _player != null ? _player : FindObjectOfType<PlayerController>();
    public static bool IsGameOver { get; private set; }

    public Timer Timer { get; private set; }

    void Start()
    {
        _currentScene = SceneManager.GetActiveScene();
        Timer = new();
    }

    void Update()
    {
        Timer.Tick(Time.deltaTime);
        
        if (RotateLeft)
            CameraController.Instance.RotateLeft();
        if (RotateRight)
            CameraController.Instance.RotateRight();
        
        if (ChangeInputs)
            useController = !useController;

        if (Input.GetKeyDown(KeyCode.R)) ReloadScene();
    }
    public void ReloadScene()
    {
        if (fading) return;

        fading = true;
        LoadingManager.Instance.FadeIn(() => {
            LoadingManager.Instance.SetLoading(true)
                .LoadScene( /*TODO: Fix Hacky Solution(criminal)*/(SceneType)_currentScene.buildIndex);
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
    public static T GetGlobalInstance<T>(string key) where T : Object
    {
        return GlobalInstancesBehaviour.GlobalInstances.GetInstance<T>(key);
    }
}