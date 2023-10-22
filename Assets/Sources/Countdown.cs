using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources
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

            // foreach (var o in toDisable)
            // {
            //     o.SetActive(false);
            // }

            GameManager.Instance.ShowEndGame();
        }
    }
}