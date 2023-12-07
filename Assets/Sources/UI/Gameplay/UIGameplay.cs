using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] GameObject keysPanel;
    [SerializeField] ButtonBehaviour settingsBtn;
    [SerializeField] UIUpgrades uiUpgrades;
    [SerializeField] Spawner spawner;

    void Start()
    {
        settingsBtn.AddListener(() =>
        {
            GameLogger.Log("Open settings");
        }, InteractionType.ClickUp);

        // var spawner = GameManager.GetGlobalInstance<Spawner>("spawner");
    }

    void OnEnable()
    {
        spawner.onAllEnemiesDead += ShowUpgrades;
    }

    void OnDisable()
    {
        spawner.onAllEnemiesDead -= ShowUpgrades;
    }

    void ShowKeys()
    {
        keysPanel.SetActive(true);
        Helpers.Delay(5f, () => keysPanel.SetActive(false));
    }

    void ShowUpgrades()
    {
        uiUpgrades.Enable();
    }
}
