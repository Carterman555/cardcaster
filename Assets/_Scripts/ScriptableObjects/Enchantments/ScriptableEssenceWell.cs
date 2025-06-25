public class ScriptableEssenceWell : ScriptableEnchantment {

    public override void OnGain() {
        base.OnGain();
        DeckManager.Instance.UpdateMaxEssence();
    }

}
