using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] AudioClip purchaseAudio;

    public event System.Action OnUpgradeDisabled;

    bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled) return;
            
            _enabled = value;
            upgradeBtn.interactable = value;

            if (!value)
                OnUpgradeDisabled?.Invoke();
        }
    }

    int level => Progress.Instance.upgrades.GetLevel(upgradeType);
    int cost => (int)costCurve.Evaluate(level);

    Currency playerCurrency => Progress.Instance.currency;
    Upgrades playerUpgrades => Progress.Instance.upgrades;

    bool MAXLEVEL => level == 10;

    public void OnEnable()
    {
        UpdateInfos(upgradeType, playerUpgrades.GetLevel(upgradeType));
        playerUpgrades.OnUpgrade += UpdateInfos;
    }

    void OnDisable()
    {
        playerUpgrades.OnUpgrade -= UpdateInfos;
    }

    void Start()
    {
        upgradeBtn.onClick.AddListener(Upgrade);
    }

    public void Upgrade()
    {
        playerCurrency.money -= cost;
        Progress.Instance.upgrades.Upgrade(upgradeType);

        if (purchaseAudio != null)
            purchaseAudio.Play();
    }

    public void Select() => upgradeBtn.Select();

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

        Enabled = playerCurrency.money >= cost;
    }
}