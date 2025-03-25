using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPersistentCard", menuName = "Cards/Persistent Card")]
public class ScriptablePersistentCard : ScriptableCardBase {

    public event Action<int> OnLevelUp;

    public int MaxLevel => maxLevel;
    public int CurrentLevel { get; private set; }

    [SerializeField] private int maxLevel;
    [SerializeField] private PlayerStatModifier[] statModifiersPerLevel;

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    // upgrade card
    protected override void Play(Vector2 position) {
        base.Play(position);
        if (CurrentLevel < maxLevel) {
            StatsManager.Instance.AddPlayerStatModifiers(statModifiersPerLevel);
            CurrentLevel++;
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();
        CurrentLevel = 0;
    }

    public override void OnRemoved() {
        base.OnRemoved();

        // unsub from all things that subbed to it
        OnLevelUp = null;

        // remove all the stat modifiers it added
        for (int i = 0; i < CurrentLevel; i++) {
            StatsManager.Instance.RemovePlayerStatModifiers(statModifiersPerLevel);
        }
    }
}
