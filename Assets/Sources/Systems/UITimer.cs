using TMPro;
using UnityEngine;

namespace Sources.Systems
{
    public class UITimer : MonoBehaviour
    {
        public TextMeshProUGUI timerTMPRO;
        float curTimer => GameManager.Instance.Timer;
        void Update()
        {
            timerTMPRO.text = curTimer.ToString("0");
        }
    }
}