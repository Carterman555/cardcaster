using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPermCard", menuName = "Cards/Perm Card")]
public class ScriptablePermCard : ScriptableCardBase {

    [SerializeField] private int maxLevel;
    private int currentLevel;

    [SerializeField] private PlayerStatsModifier statsModifierPerLevel;
    private List<PlayerStatsModifier> appliedStatsModifiers;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    // upgrade card
    protected override void Play(Vector2 position) {
        base.Play(position);

        currentLevel++;
        currentLevel = Mathf.Min(currentLevel, maxLevel);

        if (currentLevel > appliedStatsModifiers.Count) {
            StatsManager.Instance.AddPlayerStatsModifier(statsModifierPerLevel);
            appliedStatsModifiers.Add(statsModifierPerLevel);
        }
    }

    private void OnEnable() {
        appliedStatsModifiers = new();
        currentLevel = 0;
    }

    private void OnDestroy() {
        foreach (var modifier in appliedStatsModifiers) {
            StatsManager.Instance.RemovePlayerStatsModifier(statsModifierPerLevel);
        }
        Debug.Log("Remove all stat modifiers from: " + Name);
    }
}
