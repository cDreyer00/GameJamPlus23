using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISound : MonoBehaviour
{
    [SerializeField] Color enabledColor;
    [SerializeField] Color disabledColor;
    [SerializeField] Image img;

    bool _enabled;
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            img.color = _enabled ? enabledColor : disabledColor;
        }
    }

    void Awake()
    {
        img = img != null ? img : GetComponent<Image>();
        Button btn = GetComponent<Button>() != null ? GetComponent<Button>() : gameObject.AddComponent<Button>();
        btn.onClick.AddListener(() => SoundManager.Instance.SoundEnabled = !SoundManager.Instance.SoundEnabled);
    }

    void Start()
    {
        Enabled = SoundManager.Instance.SoundEnabled;
        SoundManager.Instance.OnSoundEnabled += (enabled) => Enabled = enabled;
    }
}
