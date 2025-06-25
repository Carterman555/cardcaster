public class ScriptableSagesWisdom : ScriptableEnchantment {

    public override void OnGain() {
        base.OnGain();

        DeckManager.Instance.UpdateHandSize();
    }

}
