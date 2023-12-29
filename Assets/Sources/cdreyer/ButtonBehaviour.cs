using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public enum ButtonAnimation { Shake, Punch, Yoyo }
public enum InteractionType
{
    ClickUp, ClickDown, Enter,
    Exit
}

public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public           bool              interactable = true;
    [SerializeField] FeedbackActions[] feedbackActions;
    public FeedbackActions[] FeedbackActions => feedbackActions;

    public ScriptableObjectEvent onClickDown;
    public ScriptableObjectEvent onClickUp;
    public ScriptableObjectEvent onEnter;
    public ScriptableObjectEvent onExit;

    bool    dragging;
    Vector3 inputPos = Vector3.zero;

    public RectTransform RectTransform { get; private set; }
    public Image Image { get; private set; }


    void OnValidate()
    {
        if (!onClickDown) onClickDown = ScriptableObject.CreateInstance<ScriptableObjectEvent>();
        if (!onClickUp) onClickUp = ScriptableObject.CreateInstance<ScriptableObjectEvent>();
        if (!onEnter) onEnter = ScriptableObject.CreateInstance<ScriptableObjectEvent>();
        if (!onExit) onExit = ScriptableObject.CreateInstance<ScriptableObjectEvent>();
    }

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
    }
    public void OnPointerUp(PointerEventData eventData) => ExecuteInteractions(InteractionType.ClickUp);
    public void OnPointerDown(PointerEventData eventData)
    {
        inputPos = Input.mousePosition;
        ExecuteInteractions(InteractionType.ClickDown);
    }
    public void OnPointerEnter(PointerEventData eventData) => ExecuteInteractions(InteractionType.Enter);
    public void OnPointerExit(PointerEventData eventData) => ExecuteInteractions(InteractionType.Exit);

    public void AddListener(Action action, InteractionType interaction)
    {
        switch (interaction) {
        case InteractionType.ClickDown:
            onClickDown.AddListener(action);
            return;
        case InteractionType.ClickUp:
            onClickUp.AddListener(action);
            return;
        case InteractionType.Enter:
            onEnter.AddListener(action);
            return;
        case InteractionType.Exit:
            onExit.AddListener(action);
            return;
        }
    }
    public bool RemoveListener(Action action, InteractionType interaction)
    {
        return interaction switch
        {
            InteractionType.ClickDown => onClickDown.RemoveListener(action),
            InteractionType.ClickUp   => onClickUp.RemoveListener(action),
            InteractionType.Enter     => onEnter.RemoveListener(action),
            InteractionType.Exit      => onExit.RemoveListener(action),
            _                         => false
        };
    }
    public void ClearListeners()
    {
        onClickDown.RemoveAllListeners();
        onClickUp.RemoveAllListeners();
        onEnter.RemoveAllListeners();
        onExit.RemoveAllListeners();
    }

    void RunFeedback(FeedbackActions feedback)
    {
        switch (feedback.buttonFeedback) {
        case ButtonAnimation.Shake:
            RectTransform.DOShakeAnchorPos(0.5f, 1);
            break;
        case ButtonAnimation.Punch:
            RectTransform.DOPunchAnchorPos(new Vector2(0, 1), 1);
            break;
        case ButtonAnimation.Yoyo:
            RectTransform.DOScale(0.5f, 2).SetEase(Ease.InOutSine);
            break;
        default:
            break;
        }
    }

    void ExecuteInteractions(InteractionType interaction)
    {
        if (!CanRunInteraction()) {
            OnInteractionDeclined(interaction);
            return;
        }

        foreach (FeedbackActions feedback in FeedbackActions) {
            if (feedback.declinedInteraction) continue;
            if (feedback.interactionType != interaction) continue;

            // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

            RunFeedback(feedback);
        }

        switch (interaction) {
        case InteractionType.ClickDown:
            onClickDown.Invoke(this, default(object));
            break;
        case InteractionType.ClickUp:
            onClickUp.Invoke(this, default(object));
            break;
        case InteractionType.Enter:
            onEnter.Invoke(this, default(object));
            break;
        case InteractionType.Exit:
            onExit.Invoke(this, default(object));
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(interaction), interaction, null);
        }
    }

    void OnInteractionDeclined(InteractionType interaction)
    {
        foreach (FeedbackActions feedback in FeedbackActions) {
            if (!feedback.declinedInteraction) continue;
            if (feedback.interactionType != interaction) continue;

            // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

            RunFeedback(feedback);
        }
    }


    bool CanRunInteraction()
    {
        if (!interactable) return false;
        if (inputPos != Vector3.zero) {
            float dist = Vector3.Distance(inputPos, Input.mousePosition);
            if (dist > 10f) return false;
        }

        return true;
    }
    
}

[Serializable]
public class FeedbackActions
{
    public ButtonAnimation buttonFeedback;
    public InteractionType interactionType;
    public AudioClip       audioClip;

    [Tooltip("triggers the feedback when the interactable is false")] public bool declinedInteraction;
}