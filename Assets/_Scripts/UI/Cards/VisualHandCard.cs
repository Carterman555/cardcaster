using MoreMountains.Feedbacks;
using UnityEngine;

public class VisualHandCard : MonoBehaviour {

    private CanvasGroup canvasGroup;
    [SerializeField] private CardImage cardImage;

    [SerializeField] private MMF_Player duplicateCardPlayer;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable() {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one;
    }

    public void PlayDuplicateCardVisual(HandCard duplicatingHandCard) {

        canvasGroup.alpha = 1f;

        transform.position = duplicatingHandCard.transform.position;

        cardImage.Setup(duplicatingHandCard.GetCard());

        duplicateCardPlayer.PlayFeedbacks();
        duplicateCardPlayer.Events.OnComplete.AddListener(Return);
    }

    private void Return() {
        duplicateCardPlayer.Events.OnComplete.RemoveListener(Return);
        gameObject.ReturnToPool();
    }

}
