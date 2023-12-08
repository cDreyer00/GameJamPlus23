using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWaves : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpro;
    [SerializeField] string strFormat = "Wave {0}";

    Spawner spawner => GameManager.GetGlobalInstance<Spawner>("spawner");

    void OnValidate()
    {
        if (tmpro == null)
            tmpro = GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        GameManager.GetGlobalInstance<Spawner>("spawner", 0.2f, (s) =>
        {            
            s.onBattleCompleted += OnComplete;
        });
    }

    void OnDisable()
    {
        spawner.onBattleCompleted -= OnComplete;
    }

    void OnComplete()
    {
        tmpro.text = "Completed, press R to restart";
    }

    void Update()
    {
        if (spawner == null) return;
        if (spawner.IsCompleted) return;
        int remaingWaves = spawner.RemainingWaves;
        tmpro.text = string.Format(strFormat, remaingWaves);
    }
}
