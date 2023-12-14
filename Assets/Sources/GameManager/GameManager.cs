using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] MeshRenderer groundMr;
    //[SerializeField] Spawner      spawner;
    [SerializeField] ScriptableObjectEvent gameOverEvent;
    public           bool                  useController = true;
    float                                  _initTime;
    public bool                            fading;

    //public Bounds GameBounds => groundMr == null ? new(Vector3.zero, Vector3.zero) : groundMr.bounds;
    //public Spawner Spawner => spawner;

    bool RotateLeft => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Q);
    bool RotateRight => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.E);
    bool ChangeInputs => Input.GetKeyDown(KeyCode.Space);

    public delegate bool TimerPauseVerifier();
    public event TimerPauseVerifier OnTimerPauseCheck;

    public bool IsTimerPaused
    {
        get
        {
            if (OnTimerPauseCheck != null) {
                foreach (var verifier in OnTimerPauseCheck.GetInvocationList()) {
                    if (((TimerPauseVerifier)verifier)()) {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    Character _player;
    public Character Player => _player = _player != null ? _player : FindObjectOfType<PlayerController>();
    //public static bool IsGameOver { get; private set; }

    public Timer Timer { get; private set; } = new();
    void Update()
    {
        if (!IsTimerPaused)
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

        DeactivateSceneExceptCameraNode(SceneManager.GetActiveScene());
        LoadingManager.Instance.FadeIn(() => {
            LoadingManager.Instance
                .SetLoading(true)
                .LoadScene(SceneType.GAMEPLAY);
            //IsGameOver = false;
            fading = false;
        });
        SoundManager.Instance.Stop();
        Progress.Instance.Clear();
        Progress.Instance.Load();
    }
    public void GameOver()
    {
        gameOverEvent.Invoke(this, null);
    }
    static void DeactivateSceneExceptCameraNode(Scene scene)
    {
        var rootGameObjects = scene.GetRootGameObjects();
        foreach (var go in rootGameObjects) {
            if (go.GetComponentInChildren<Camera>()) continue;
            if(go == Instance.gameObject) continue;
            go.SetActive(false);
        }
    }
    public static void GetGlobalInstance<T>(string key, float timeout, Action<T> callback) where T : UnityEngine.Object
    {
        Instance.StartCoroutine(GetGlobalInstanceCoroutine(key, timeout, callback));

        static IEnumerator GetGlobalInstanceCoroutine(string key, float timeout, Action<T> callback)
        {
            float startTime = Time.time;
            T     instance  = null;

            while (Time.time - startTime < timeout) {
                instance = GlobalInstancesBehaviour.GlobalInstances.GetInstance<T>(key);
                if (instance != null) {
                    break;
                }
                yield return null; // wait for next frame
            }

            callback(instance);
        }
    }

    public static T GetGlobalInstance<T>(string key) where T : UnityEngine.Object
    {
        return GlobalInstancesBehaviour.GlobalInstances.GetInstance<T>(key);
    }
}