using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public EventListener onClickDown;
    public EventListener onClickUp;
    public EventListener onEnter;
    public EventListener onExit;

    bool    dragging;
    Vector3 inputPos = Vector3.zero;

    public RectTransform RectTransform { get; private set; }
    public Image Image { get; private set; }

    void OnValidate()
    {
        if (onClickDown.Equals(default)) onClickDown = EventListener.CreateInstance();
        if (onClickUp.Equals(default)) onClickUp = EventListener.CreateInstance();
        if (onEnter.Equals(default)) onEnter = EventListener.CreateInstance();
        if (onExit.Equals(default)) onExit = EventListener.CreateInstance();
    }
    private void Awake()
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
            onClickDown.eventSource.AddListener(action);
            return;
        case InteractionType.ClickUp:
            onClickUp.eventSource.AddListener(action);
            return;
        case InteractionType.Enter:
            onEnter.eventSource.AddListener(action);
            return;
        case InteractionType.Exit:
            onExit.eventSource.AddListener(action);
            return;
        }
    }
    public void RemoveListener(Action action, InteractionType interaction)
    {
        switch (interaction) {
        case InteractionType.ClickDown:
            onClickDown.eventSource.RemoveListener(action);
            return;
        case InteractionType.ClickUp:
            onClickUp.eventSource.RemoveListener(action);
            return;
        case InteractionType.Enter:
            onEnter.eventSource.RemoveListener(action);
            return;
        case InteractionType.Exit:
            onExit.eventSource.RemoveListener(action);
            return;
        }
    }

    public void ClearListeners()
    {
        onClickDown.eventSource.RemoveAllListeners();
        onClickUp.eventSource.RemoveAllListeners();
        onEnter.eventSource.RemoveAllListeners();
        onExit.eventSource.RemoveAllListeners();
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

        ScriptableObjectEvent scriptableObjectEvent = interaction switch
        {
            InteractionType.ClickDown => onClickDown.eventSource,
            InteractionType.ClickUp   => onClickUp.eventSource,
            InteractionType.Enter     => onEnter.eventSource,
            InteractionType.Exit      => onExit.eventSource,
            _                         => throw new ArgumentOutOfRangeException(nameof(interaction), interaction, null)
        };
        scriptableObjectEvent.Invoke(this, null);
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