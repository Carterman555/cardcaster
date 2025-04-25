using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityIndicator : MonoBehaviour {

    [SerializeField] private Image iconImage;
    [SerializeField] private Image durationLeftImage;

    public CardType CardType { get; private set; }

    private float totalDuration;
    private float durationLeft;

    private bool completed;
    private float SCALE_DURATION = 0.15f;

    public void Setup(ScriptableAbilityCardBase abilityCard) {
        CardType = abilityCard.CardType;
        iconImage.sprite = abilityCard.Sprite;
        durationLeftImage.fillAmount = 1f;

        totalDuration = abilityCard.Stats.Duration;
        durationLeft = abilityCard.Stats.Duration;

        transform.DOKill();
        transform.DOScale(1f, SCALE_DURATION);

        completed = false;
    }

    private void Update() {

        if (completed) {
            return;
        }

        if (durationLeft < 0f) {
            AbilityIndicatorManager.Instance.RemoveIndicatorFromList(this);

            transform.DOKill();
            transform.DOScale(0f, SCALE_DURATION).OnComplete(() => {
                gameObject.ReturnToPool();
            });

            completed = true;
        }

        durationLeft -= Time.deltaTime;
        durationLeftImage.fillAmount = durationLeft / totalDuration;
    }

    public void ResetDuration(ScriptableAbilityCardBase abilityCard) {
        totalDuration = abilityCard.Stats.Duration; // set duration again because modifiers could have made it different

        durationLeft = totalDuration;
    }
}
