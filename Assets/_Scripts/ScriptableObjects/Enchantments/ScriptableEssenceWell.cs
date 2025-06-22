using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableEssenceWell : ScriptableEnchantment {

    public override void OnGain() {
        base.OnGain();
        DeckManager.Instance.UpdateMaxEssence();
    }

}
