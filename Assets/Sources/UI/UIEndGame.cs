using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CDreyer;
using Microsoft.Unity.VisualStudio.Editor;

public class UIEndGame : MonoBehaviour
{
    [SerializeField] ButtonBehaviour btn1;

    void Start()
    {
        GameManager.Instance.ReloadScene();
    }
}
