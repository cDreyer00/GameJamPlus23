using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIUpgrades : MonoBehaviour
{
    [SerializeField] Button continueBtn;

    void Start()
    {
        continueBtn.onClick.AddListener(() =>
        {
            Disable();
            var spawner = GameManager.GetGlobalInstance<Spawner>("spawner");
            spawner.StartWave();
        });
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
