using System;
using DG.Tweening;
using Plugins.Demigiant.DOTween.Modules;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sources.cdreyer.loading_system
{
    public class LoadingManager : Singleton<LoadingManager>
    {
        public bool isLoading { get; private set; }
        public SceneType currentScene { get; private set; }

        [SerializeField] Image fade;
        [SerializeField] float fadeTimer;
        [SerializeField] GameObject loadingPanel;
        [SerializeField] bool loadingOnAwake;

        override protected void Awake()
        {
            base.Awake();

            SetLoading(loadingOnAwake);

            currentScene = CurrentOpenSceneAsSceneType();
            fade.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        SceneType CurrentOpenSceneAsSceneType()
        {
            return (SceneType)SceneManager.GetActiveScene().buildIndex;
        }

        public void LoadScene(SceneType scene, Action onComplete = null)
        {
            SceneManager.LoadScene((int)scene);

            UnityAction<Scene, LoadSceneMode> sceneLoadedHandler = null;
            sceneLoadedHandler = (s, mode) =>
            {
                currentScene = scene;
                onComplete?.Invoke();

                // Unsubscribe from the event to avoid memory leaks.
                SceneManager.sceneLoaded -= sceneLoadedHandler;
            };

            SceneManager.sceneLoaded += sceneLoadedHandler;

        }

        public void LoadSceneAsync(SceneType scene, Action onComplete = null)
        {
            currentScene = scene;
            AsyncOperation operation = SceneManager.LoadSceneAsync((int)scene);
            operation.completed += (op) =>
            {
                onComplete?.Invoke();
            };
        }

        public void FadeIn(Action onComplete)
        {
            fade.gameObject.SetActive(true);

            fade.color = new Color(0, 0, 0, 0);

            Sequence s = DOTween.Sequence();
            s.Append(
                fade.DOColor(new Color(0, 0, 0, 1), 0.25f)
                    .SetUpdate(true)
            ).SetUpdate(true);
            s.OnComplete(() =>
            {
                onComplete?.Invoke();
                fade.gameObject.SetActive(false);
            });
        }

        public LoadingManager SetLoading(bool loading)
        {
            isLoading = loading;
            loadingPanel.SetActive(loading);

            return this;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetLoading(false);
        }
    }

    public enum SceneType
    {
        GAMEPLAY = 0
    }
}