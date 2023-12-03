using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class UIUpgrades : MonoBehaviour
{
    [SerializeField] Button continueBtn;
    [SerializeField] UIAttribute[] uiAttributes;

    void OnValidate()
    {
        if (uiAttributes.Length == 0)
            uiAttributes = GetComponentsInChildren<UIAttribute>();
    }

    void Start()
    {
        continueBtn.onClick.AddListener(() =>
        {
            Disable();
            var spawner = GameManager.GetGlobalInstance<Spawner>("spawner");
            spawner.StartWave();
        });

        foreach (var attr in uiAttributes)
        {
            attr.OnUpgradeDisabled += () =>
            {
                var firstAvailable = uiAttributes.FirstOrDefault(a => a.Enabled);
                if (firstAvailable != null)
                    firstAvailable.Select();
                else
                    continueBtn.Select();
            };
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        foreach (var uiattr in uiAttributes)
        {
            if (!uiattr.Enabled) continue;
            uiattr.Select();
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
