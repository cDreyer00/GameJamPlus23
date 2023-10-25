using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;
using Unity.VisualScripting;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] GameObject keysPanel;
    [SerializeField] ButtonBehaviour settingsBtn;

    void Start()
    {
        Helpers.ActionCallbackCr(() => keysPanel.SetActive(false), 5);

        settingsBtn.AddListener(() =>
        {
            GameLogger.Log("Open settings");
        }, InteractionType.ClickUp);
    }
}