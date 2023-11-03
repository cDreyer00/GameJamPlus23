using TMPro;
using UnityEngine;

namespace Sources.Systems
{
    public class Countdown : MonoBehaviour
    {
        public float time;

        //public GameObject[] toDisable;
        public TextMeshProUGUI textMesh;

        private void Start()
        {
            textMesh = textMesh == null ? GetComponent<TextMeshProUGUI>() : textMesh;
            textMesh.text = time.ToString("0");
        }

        void Update()
        {
            time -= Time.deltaTime;
            textMesh.text = time.ToString("0");
            if (!(time <= 0)) return;
            if (GameManager.GameManager.IsGameOver) return;
            GameManager.GameManager.Instance.ShowEndGame();
        }
    }
}