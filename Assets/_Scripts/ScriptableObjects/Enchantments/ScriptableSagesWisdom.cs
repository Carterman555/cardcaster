using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableSagesWisdom : ScriptableEnchantment {

    public override void OnGain() {
        base.OnGain();

        DeckManager.Instance.UpdateHandSize();
    }

}
