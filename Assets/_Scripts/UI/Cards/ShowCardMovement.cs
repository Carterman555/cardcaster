using DG.Tweening;
using UnityEngine;

public class ShowCardMovement : MonoBehaviour {

    public enum ShowState { None, Hidden, Showing, Moving }
    private ShowState showState;
    private ShowState delayedCommand;

    private RectTransform rectTransform;

    private Tween moveTween;

    private Vector2 hidePos, showPos;

    [SerializeField] private CanvasGroup canvasGroup;

    private HandCard handCard;

    const float duration = 0.2f;
    const float fade = 0.7f;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        handCard = GetComponent<HandCard>();
    }

    public void Setup(Vector2 hidePos, Vector2 showPos) {
        this.hidePos = hidePos;
        this.showPos = showPos;

        delayedCommand = ShowState.None;
        showState = ShowState.Hidden;

        canvasGroup.alpha = fade;
    }

    [ContextMenu("Show")]
    public void Show() {

        if (!enabled) {
            return;
        }

        if (showState == ShowState.Hidden) {
            showState = ShowState.Moving;

            moveTween = rectTransform.DOAnchorPos(showPos, duration);
            canvasGroup.DOFade(1f, duration).OnComplete(() => {
                showState = ShowState.Showing;
            });
        }
        else if (showState == ShowState.Moving) {
            delayedCommand = ShowState.Showing;
        }
    }

    [ContextMenu("Hide")]
    public void Hide() {

        if (!enabled) {
            return;
        }

        if (showState == ShowState.Showing) {
            showState = ShowState.Moving;

            moveTween = rectTransform.DOAnchorPos(hidePos, duration);
            canvasGroup.DOFade(fade, duration).OnComplete(() => {
                showState = ShowState.Hidden;
                handCard.CurrentCardState = HandCard.CardState.ReadyToPlay;
            });
        }
        else if (showState == ShowState.Moving) {
            delayedCommand = ShowState.Hidden;
        }
    }

    public void OnPositioningCard() {
        showState = ShowState.Showing;
        canvasGroup.DOFade(1f, duration);
    }
    
    private void Update() {

        if (delayedCommand != ShowState.None) {
            if (showState != ShowState.Moving) {

                if (delayedCommand == ShowState.Hidden) {
                    Hide();
                }
                else if (delayedCommand == ShowState.Showing) {
                    Show();
                }

                delayedCommand = ShowState.None;
            }
        }
    }
}
