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
        public ButtonBehaviour[] behaviours;

        [SerializeField] Canvas canvas;

        Action _showCanvas;
        void OnValidate()
        {
            if (!canvas) canvas = GetComponentInChildren<Canvas>();
        }
        void Awake()
        {
            _showCanvas = ShowCanvas;
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
            GameEvents.OnGameOver.AddListener(_showCanvas);
        }
        void OnDisable()
        {
            if (!GameEvents.OnGameOver.RemoveListener(_showCanvas)) {
                Debug.LogWarning("Could not remove listener from gameOverEvent");
            }
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