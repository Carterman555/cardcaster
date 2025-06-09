using UnityEngine;

[CreateAssetMenu(fileName = "OpenPalmsCard", menuName = "Cards/Open Palms Card")]
public class ScriptableOpenPalmsCard : ScriptablePersistentCard {

    protected override void Play(Vector2 position) {
        base.Play(position);
        DeckManager.Instance.UpdateHandSize();
    }
}

