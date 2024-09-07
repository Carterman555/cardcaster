using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Stats Modifier")]
public class ScriptableStatsModifierItem : ScriptableItemBase {

    [SerializeField] private PlayerStatsModifier statsModifier;

    public override void Activate() {
        StatsManager.Instance.AddPlayerStatsModifier(statsModifier);
    }

    public override void Deactivate() {
        StatsManager.Instance.RemovePlayerStatsModifier(statsModifier);
    }
}
