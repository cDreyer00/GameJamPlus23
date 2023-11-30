using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIUpgrades : MonoBehaviour
{
    public void Upgrade()
    {
        int coins = Progress.Instance.currency;
        int cost = 999;
        if (coins < cost) return;
        Progress.Instance.currency.money -= cost;
    }
}
