using System;
using System.Collections.Generic;
using DG.Tweening;
using Sources.cdreyer;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Sources.UI
{
    public class UIEndGame : MonoBehaviour
    {
        public enum ButtonType { Restart = 0, Menu = 1, }

        public ButtonBehaviour[] behaviours;

        [SerializeField] Canvas                canvas;
        [SerializeField] ScriptableObjectEvent gameOverEvent;

        Action _showCanvas;
        Action _reloadScene;
        void OnValidate()
        {
            if (!canvas) canvas = GetComponentInChildren<Canvas>();
        }
        void Awake()
        {
            _showCanvas = ShowCanvas;
            _reloadScene = GameManager.Instance.ReloadScene;
        }
        void ShowCanvas()
        {
            var gameManager = GameManager.Instance;
            if (gameManager.fading) return;
            gameManager.fading = true;
            LoadingManager.Instance.FadeIn(() => canvas.enabled = true);
            SoundManager.Instance.Stop();
            gameManager.fading = false;
        }
        void Start()
        {
            foreach (var b in behaviours) {
                b.transform.localScale = Vector3.zero;
                b.transform.DOScale(Vector3.one, 0.5f);
            }
        }
        void OnEnable()
        {
            canvas.enabled = false;
            gameOverEvent.AddListener(_showCanvas);
            behaviours[(int)ButtonType.Restart].AddListener(_reloadScene, InteractionType.ClickUp);
        }
        void OnDisable()
        {
            gameOverEvent.RemoveListener(_showCanvas);
            behaviours[(int)ButtonType.Restart].RemoveListener(_reloadScene, InteractionType.ClickUp);
        }
    }

    #region Editor

    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UIEndGame))]
    public class UIEndGameEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("GetBehaviorsInChildren", GUILayout.Height(30))) {
                var uiEndGame = (UIEndGame)target;
                uiEndGame.behaviours = uiEndGame.GetComponentsInChildren<ButtonBehaviour>();
            }
            base.OnInspectorGUI();
        }
    }
    #endif

    #endregion
}