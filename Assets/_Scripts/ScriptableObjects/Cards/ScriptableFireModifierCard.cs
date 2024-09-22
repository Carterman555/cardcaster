using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireModifierCard", menuName = "Cards/Modifiers/Fire")]
public class ScriptableFireModifierCard : ScriptableModifierCardBase {

    protected override void ApplyToAbility() {
        base.ApplyToAbility();
    }

    protected override void ApplyVisualEffect(Transform targetForVisual) {
        base.ApplyVisualEffect(targetForVisual);


    }
}
