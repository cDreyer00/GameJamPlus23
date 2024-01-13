using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInput : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite controllerSprite;
    [SerializeField] Sprite keyboardSprite;

    bool _useController;

    void OnValidate()
    {
        if (image == null)
            image = transform.GetChild(0).GetComponent<Image>();
    }

    void Awake(){
        Button button = GetComponent<Button>() != null ? GetComponent<Button>() : gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.useController = !GameManager.Instance.useController);
    }

    void Update()
    {
        if(IsDirty(GameManager.Instance.useController))
        {
            _useController = GameManager.Instance.useController;
            image.sprite = _useController ? controllerSprite : keyboardSprite;
        }
    }

    bool IsDirty(bool prev) => prev != _useController;
}
