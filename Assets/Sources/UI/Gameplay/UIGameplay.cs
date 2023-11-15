using UnityEngine;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] GameObject keysPanel;
    [SerializeField] ButtonBehaviour settingsBtn;

    void Start()
    {
        Helpers.Delay(5f, () => keysPanel.SetActive(false));

        settingsBtn.AddListener(() =>
        {
            GameLogger.Log("Open settings");
        }, InteractionType.ClickUp);
    }
}