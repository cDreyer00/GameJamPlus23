using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;
using DG.Tweening;

public class UIEndGame : MonoBehaviour
{
    [SerializeField] ButtonBehaviour[] btns;

    void Start()
    {
        foreach (var b in btns)
        {
            b.transform.localScale = Vector3.zero;
            b.transform.DOScale(Vector3.one, 0.5f);
            b.AddListener(GameManager.PlayerHealthBar.ReloadScene, InteractionType.ClickUp);
        };
    }
}
