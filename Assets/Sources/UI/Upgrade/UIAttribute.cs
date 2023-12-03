using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Progress;
using static Progress.Upgrades;

public class UIAttribute : MonoBehaviour
{
    [SerializeField] Type upgradeType;
    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI costTxt;
    [SerializeField] Button upgradeBtn;
    [Space]
    [SerializeField] AnimationCurve costCurve;

    int level => Progress.Instance.upgrades.GetLevel(upgradeType);
    int cost => (int)costCurve.Evaluate(level);

    Currency playerCurrency => Progress.Instance.currency;
    Upgrades playerUpgrades => Progress.Instance.upgrades;

    bool MAXLEVEL => level == 10;

    public void OnEnable()
    {
        UpdateInfos(upgradeType, playerUpgrades.GetLevel(upgradeType));
    }

    void Start()
    {
        upgradeBtn.onClick.AddListener(Upgrade);
        playerUpgrades.OnUpgrade += UpdateInfos;
    }

    public void Upgrade()
    {
        playerCurrency.money -= cost;
        Progress.Instance.upgrades.Upgrade(upgradeType);
    }

    void UpdateInfos(Type uprType, int level) => UpdateInfos();
    void UpdateInfos()
    {
        if (MAXLEVEL)
        {
            levelTxt.text = "MAX";
            costTxt.gameObject.SetActive(false);
            upgradeBtn.interactable = false;
            return;
        }

        costTxt.text = $"${cost}";
        levelTxt.text = $"lv{level}";

        upgradeBtn.interactable = playerCurrency.money >= cost;
    }
}