using UnityEngine;
using UnityEngine.UI;

public class AbilityIndicator : MonoBehaviour {

    [SerializeField] private Image iconImage;
    [SerializeField] private Image durationLeftImage;

    public CardType CardType { get; private set; }

    private float totalDuration;
    private float durationLeft;

    public void Setup(ScriptableAbilityCardBase abilityCard) {
        CardType = abilityCard.CardType;
        iconImage.sprite = abilityCard.Sprite;
        durationLeftImage.fillAmount = 1f;

        totalDuration = abilityCard.Stats.Duration;
        durationLeft = abilityCard.Stats.Duration;
    }

    private void Update() {
        if (durationLeft < 0f) {
            AbilityIndicatorManager.Instance.RemoveIndicatorFromList(this);
            gameObject.ReturnToPool();
        }

        durationLeft -= Time.deltaTime;
        durationLeftImage.fillAmount = durationLeft / totalDuration;
    }

    public void ResetDuration(ScriptableAbilityCardBase abilityCard) {
        totalDuration = abilityCard.Stats.Duration; // set duration again because modifiers could have made it different

        durationLeft = totalDuration;
    }
}
