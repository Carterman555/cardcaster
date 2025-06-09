using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ImaginaryTwinCard", menuName = "Cards/Imaginary Twin Card")]
public class ScriptableImaginaryTwinCard : ScriptableAbilityCardBase {

    protected override void Play(Vector2 position) {
        base.Play(position);
        ScriptableCardBase[] cardsInHand = DeckManager.Instance.GetCardsInHand().Where(c => c != this).ToArray();

        ScriptableCardBase randomCard = cardsInHand.RandomItem();
        DeckManager.Instance.GainCard(randomCard);
    }

}
