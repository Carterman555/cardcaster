using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WisdomsHoldCard", menuName = "Cards/Wisdom's Hold Card")]
public class ScriptableWisdomsHoldCard : ScriptablePersistentCard {

    protected override void Play(Vector2 position) {
        base.Play(position);
        DeckManager.Instance.UpdateHandSize();
    }
}

