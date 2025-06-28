using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ImaginaryTwinCard", menuName = "Cards/Imaginary Twin Card")]
public class ScriptableImaginaryTwinCard : ScriptableAbilityCardBase {

    [SerializeField] private VisualHandCard visualHandCardPrefab;

    protected override void Play(Vector2 position) {
        base.Play(position);

        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand().Where(c => c != this).ToArray();

        ScriptableCardBase randomCard = cardsInHand.RandomItem();
        ScriptableCardBase cardDuplicate = ResourceSystem.Instance.GetCardInstance(randomCard.CardType);
        DeckManager.Instance.GainCard(cardDuplicate);

        if (CardsUIManager.Instance.TryGetHandCard(randomCard, out HandCard randomHandCard)) {
            VisualHandCard visualHandCard = visualHandCardPrefab.Spawn(CardsUIManager.Instance.transform);
            visualHandCard.PlayDuplicateCardVisual(randomHandCard);
        }
        else {
            Debug.LogError("Couldn't find hand card!");
            return;
        }
    }

}
