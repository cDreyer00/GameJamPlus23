using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] GameObject keysPanel;
    [SerializeField] ButtonBehaviour settingsBtn;
    [SerializeField] UIUpgrades uiUpgrades;

    void Start()
    {
        settingsBtn.AddListener(() =>
        {
            GameLogger.Log("Open settings");
        }, InteractionType.ClickUp);

        var spawner = GameManager.GetGlobalInstance<Spawner>("spawner");
        spawner.onAllEnemiesDead += ShowUpgrades;
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
