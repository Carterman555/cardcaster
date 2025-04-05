using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Stats Modifier")]
public class ScriptableStatsModifierCard : ScriptableAbilityCardBase {

    [SerializeField] private PlayerStatModifier[] statModifiers;
    protected PlayerStatModifier[] PlayerStatsModifier => statModifiers;

    protected override void Play(Vector2 position) {
        base.Play(position);
        StatsManager.AddPlayerStatModifiers(statModifiers);
    }

    public override void Stop() {
        base.Stop();
        StatsManager.RemovePlayerStatModifiers(statModifiers);
    }
}




