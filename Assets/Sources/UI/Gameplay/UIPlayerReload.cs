using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerReload : MonoBehaviour
{
    [SerializeField] Image img;

    public void Update()
    {
        float cur = GameManager.Instance.Player.CurDelay;
        float max = GameManager.Instance.Player.ShootDelay;
        img.fillAmount = cur / max;
    }
}
