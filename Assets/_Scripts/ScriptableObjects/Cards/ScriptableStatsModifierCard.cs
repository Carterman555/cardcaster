using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Stats Modifier")]
public class ScriptableStatsModifierCard : ScriptableAbilityCardBase {

    [SerializeField] private PlayerStatsModifier statsModifier;
    protected PlayerStatsModifier PlayerStatsModifier => statsModifier;

    protected override void Play(Vector2 position) {
        base.Play(position);
        StatsManager.Instance.AddPlayerStatsModifier(statsModifier);
    }

    public override void Stop() {
        base.Stop();
        StatsManager.Instance.RemovePlayerStatsModifier(statsModifier);
    }
}




