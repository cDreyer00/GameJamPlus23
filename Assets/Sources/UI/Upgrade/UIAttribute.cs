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
    [Space]
    [SerializeField] AnimationCurve costCurve;

    void Start()
    {
        upgradeBtn.onClick.AddListener(Upgrade);

        UpdateInfos();
    }

    public void Upgrade()
    {
        Progress.Instance.upgrades.Upgrade(upgradeType);
        UpdateInfos();
    }

    void UpdateInfos()
    {
        int level = Progress.Instance.upgrades.GetLevel(upgradeType);

        if (level == 10)
        {
            levelTxt.text = "MAX";
            costTxt.gameObject.SetActive(false);
            upgradeBtn.interactable = false;
            return;
        }


        int cost = (int)costCurve.Evaluate(level);
        costTxt.text = $"${cost}";
        levelTxt.text = $"lv{level}";
    }
}
