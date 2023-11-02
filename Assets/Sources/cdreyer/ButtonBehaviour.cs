using System;
using DG.Tweening;
using Plugins.Demigiant.DOTween.Modules;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sources.cdreyer
{
    public enum ButtonAnimation { Shake, Punch, Yoyo }
    public enum InteractionType { ClickUp, ClickDown, Enter, Exit }

    public class ButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public           bool              interactable = true;
        [SerializeField] FeedbackActions[] feedbackActions;
        public FeedbackActions[] FeedbackActions => feedbackActions;

        public Action onClickDown { get; private set; }
        public Action onClickUp { get; private set; }
        public Action onEnter { get; private set; }
        public Action onExit { get; private set; }

        bool    dragging;
        Vector3 inputPos = Vector3.zero;

        public RectTransform RectTransform { get; private set; }
        public Image Image { get; private set; }

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
            switch (interaction)
            {
            case InteractionType.ClickDown:
                onClickDown += action;
                return;
            case InteractionType.ClickUp:
                onClickUp += action;
                return;
            case InteractionType.Enter:
                onEnter += action;
                return;
            case InteractionType.Exit:
                onExit += action;
                return;
            }
        }

        public void RemoveListener(Action action, InteractionType interaction)
        {
            switch (interaction)
            {
            case InteractionType.ClickDown:
                onClickDown -= action;
                return;
            case InteractionType.ClickUp:
                onClickUp -= action;
                return;
            case InteractionType.Enter:
                onEnter -= action;
                return;
            case InteractionType.Exit:
                onExit -= action;
                return;
            }
        }

        public void ClearListeners()
        {
            onClickDown = null;
            onClickUp = null;
            onEnter = null;
            onExit = null;
        }

        void RunFeedback(FeedbackActions feedback)
        {
            switch (feedback.buttonFeedback)
            {
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
            if (!CanRunInteraction())
            {
                OnInteractionDeclined(interaction);
                return;
            }

            foreach (FeedbackActions feedback in FeedbackActions)
            {
                if (feedback.declinedInteraction) continue;
                if (feedback.interactionType != interaction) continue;

                // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

                RunFeedback(feedback);
            }

            Action action = interaction switch
            {
                InteractionType.ClickDown => onClickDown,
                InteractionType.ClickUp   => onClickUp,
                InteractionType.Enter     => onEnter,
                InteractionType.Exit      => onExit,
                _                         => null,
            };
            action?.Invoke();
        }

        void OnInteractionDeclined(InteractionType interaction)
        {
            foreach (FeedbackActions feedback in FeedbackActions)
            {
                if (!feedback.declinedInteraction) continue;
                if (feedback.interactionType != interaction) continue;

                // if (feedback.audioClip) AudioManager.PlayClip(feedback.audioClip);

                RunFeedback(feedback);
            }
        }


        bool CanRunInteraction()
        {
            if (!interactable) return false;
            if (inputPos != Vector3.zero)
            {
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

        [Tooltip("triggers the feedback when the interactable is false")]
        public bool declinedInteraction;
    }
}