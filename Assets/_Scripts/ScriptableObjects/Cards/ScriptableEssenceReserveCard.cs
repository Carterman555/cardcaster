using UnityEngine;

[CreateAssetMenu(fileName = "EssenceReserveCard", menuName = "Cards/Essence Reserve Card")]
public class ScriptableEssenceReserveCard : ScriptablePersistentCard {

    protected override void Play(Vector2 position) {
        base.Play(position);
        DeckManager.Instance.UpdateMaxEssence();
    }
}

