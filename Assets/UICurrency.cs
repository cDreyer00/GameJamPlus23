using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICurrency : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currencyTMPRO;

    int amount => Progress.Instance.currency.money;

    void Update()
    {
        if (currencyTMPRO == null) return;
        currencyTMPRO.text = amount.ToString();
    }
}
