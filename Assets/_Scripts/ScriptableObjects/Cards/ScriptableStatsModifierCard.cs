using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Stats Modifier")]
public class ScriptableStatsModifierCard : ScriptableCardBase {

    [SerializeField] private PlayerStatsModifier statsModifier;

    public override void Play(Vector2 position) {
        StatsManager.Instance.StartCoroutine(ApplyModifier());
    }

    private IEnumerator ApplyModifier() {
        StatsManager.Instance.AddPlayerStatsModifier(statsModifier);
        yield return new WaitForSeconds(effectDuration);
        StatsManager.Instance.RemovePlayerStatsModifier(statsModifier);
    }

}

[Serializable]
public class PlayerStatsModifier {

    public float MaxHealthPercent;
    public float KnockbackResistancePercent;
    public float MoveSpeedPercent;
    public int DamageIncreasePercent;
    public float AttackSpeedPercent;
    public float KnockbackStrengthPercent;
    public float DashDistancePercent;

    public static PlayerStatsModifier operator +(PlayerStatsModifier a, PlayerStatsModifier b) {
        return new PlayerStatsModifier {
            MaxHealthPercent = a.MaxHealthPercent + b.MaxHealthPercent,
            KnockbackResistancePercent = a.KnockbackResistancePercent + b.KnockbackResistancePercent,
            MoveSpeedPercent = a.MoveSpeedPercent + b.MoveSpeedPercent,
            DamageIncreasePercent = a.DamageIncreasePercent + b.DamageIncreasePercent,
            AttackSpeedPercent = a.AttackSpeedPercent + b.AttackSpeedPercent,
            KnockbackStrengthPercent = a.KnockbackStrengthPercent + b.KnockbackStrengthPercent,
            DashDistancePercent = a.DashDistancePercent + b.DashDistancePercent,
        };
    }

    public static PlayerStatsModifier operator -(PlayerStatsModifier a, PlayerStatsModifier b) {
        return new PlayerStatsModifier {
            MaxHealthPercent = a.MaxHealthPercent - b.MaxHealthPercent,
            KnockbackResistancePercent = a.KnockbackResistancePercent - b.KnockbackResistancePercent,
            MoveSpeedPercent = a.MoveSpeedPercent - b.MoveSpeedPercent,
            DamageIncreasePercent = a.DamageIncreasePercent - b.DamageIncreasePercent,
            AttackSpeedPercent = a.AttackSpeedPercent - b.AttackSpeedPercent,
            KnockbackStrengthPercent = a.KnockbackStrengthPercent - b.KnockbackStrengthPercent,
            DashDistancePercent = a.DashDistancePercent - b.DashDistancePercent,
        };
    }

    public static PlayerStatsModifier Zero = new PlayerStatsModifier {
        MaxHealthPercent = 0,
        KnockbackResistancePercent = 0,
        MoveSpeedPercent = 0,
        DamageIncreasePercent = 0,
        AttackSpeedPercent = 0,
        KnockbackStrengthPercent = 0,
        DashDistancePercent = 0
    };
}


