using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameManager : Singleton<GameManager>
{

    public bool useController = true;

    public bool fading;

    bool RotateLeft => Input.GetKeyDown(KeyCode.A)  || Input.GetKeyDown(KeyCode.Q);
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
    Action _reloadScene;
    void OnEnable()
    {
        _reloadScene ??= ReloadScene;
        GameEvents.OnRestart.AddListener(_reloadScene);
    }
    void OnDisable()
    {
        if (!GameEvents.OnRestart.RemoveListener(_reloadScene)) {
            Debug.LogWarning("Could not remove listener from restartEvent");
        }
    }

    Character _player;
    public Character Player => _player = _player != null ? _player : FindObjectOfType<PlayerController>();
    public static bool IsGameOver { get; private set; }

    public Timer Timer { get; private set; }

    void Start()
    {
        Timer = new();
    }
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
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
    void ReloadScene()
    {
        if (fading) return;
        fading = true;

        LoadingManager.Instance.FadeIn(() => {
            var root = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var go in root) {
                if (go.GetComponentInChildren<Camera>()) continue;
                go.SetActive(false);
            }
            LoadingManager.Instance.SetLoading(true)
                .LoadScene(SceneType.GAMEPLAY);
            IsGameOver = false;
            fading = false;
        });
        SoundManager.Instance.Stop();
        Progress.Instance.Clear();
        Progress.Instance.Load();
    }

    public static void GetGlobalInstance<T>(string key, float timeout, System.Action<T> callback) where T : Object
    {
        Instance.StartCoroutine(GetGlobalInstanceCoroutine(key, timeout, callback));

        static IEnumerator GetGlobalInstanceCoroutine(string key, float timeout, System.Action<T> callback)
        {
            float startTime = Time.time;
            T instance = null;

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

    public static T GetGlobalInstance<T>(string key) where T : Object
    {
        return GlobalInstancesBehaviour.GlobalInstances.GetInstance<T>(key);
    }
}