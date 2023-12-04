using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIWaves : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmpro;
    [SerializeField] string strFormat = "Wave {0}";

    void OnValidate()
    {
        if (tmpro == null)
            tmpro = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        var spawner = GameManager.GetGlobalInstance<Spawner>("spawner");
        if (spawner == null) return;
        int remaingWaves = spawner.RemainingWaves;
        tmpro.text = string.Format(strFormat, remaingWaves);
    }
}
