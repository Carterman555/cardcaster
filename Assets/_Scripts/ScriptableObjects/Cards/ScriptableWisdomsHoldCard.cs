using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WisdomsHoldCard", menuName = "Cards/Wisdom's Hold Card")]
public class ScriptableWisdomsHoldCard : ScriptablePersistentCard {

    protected override void Play(Vector2 position) {
        base.Play(position);
        DeckManager.Instance.UpdateHandSize();
    }

    // when wisdoms hold is trashed, it doesn't remove the hand size increase (base.OnRemoved would do that)
    public override void OnRemoved() {
        UnsubOnLevelUp();
    }
}

