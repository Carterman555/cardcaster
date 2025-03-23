using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPermCard", menuName = "Cards/Perm Card")]
public class ScriptablePermCard : ScriptableCardBase {

    [SerializeField] private int maxLevel;
    private int currentLevel;

    [SerializeField] private PlayerStatModifier[] statModifiersPerLevel;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    // upgrade card
    protected override void Play(Vector2 position) {
        base.Play(position);
        if (currentLevel < maxLevel) {
            StatsManager.Instance.AddPlayerStatModifiers(statModifiersPerLevel);
            currentLevel++;
        }
    }

    private void OnEnable() {
        currentLevel = 0;
    }

    private void OnRemoveCard() {
        // remove all the stat modifiers it added
        for (int i = 0; i < currentLevel; i++) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiersPerLevel);
        }
    }
}
