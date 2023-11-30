using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAttribute : MonoBehaviour
{
    [SerializeField] Progress.Upgrades.Type upgradeType;
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI costTxt;
    [SerializeField] Button upgradeBtn;

    void Start()
    {
        upgradeBtn.onClick.AddListener(Upgrade);
    }

    public void Upgrade()
    {
        Progress.Instance.upgrades.Upgrade(upgradeType);
    }
}
