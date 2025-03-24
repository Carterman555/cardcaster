using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPersistentCard", menuName = "Cards/Persistent Card")]
public class ScriptablePersistentCard : ScriptableCardBase {

    public event Action<int> OnLevelUp;

    [SerializeField] private int maxLevel;
    public int MaxLevel => maxLevel;

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
            OnLevelUp?.Invoke(currentLevel);
        }
    }

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();
        currentLevel = 0;
    }

    public override void OnRemoved() {
        base.OnRemoved();

        // unsub from all things that subbed to it
        OnLevelUp = null;

        // remove all the stat modifiers it added
        for (int i = 0; i < currentLevel; i++) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiersPerLevel);
        }
    }
}
