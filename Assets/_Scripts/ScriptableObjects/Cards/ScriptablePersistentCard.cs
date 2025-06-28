using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPersistentCard", menuName = "Cards/Persistent Card")]
public class ScriptablePersistentCard : ScriptableCardBase {

    // WARNING: prob only use for upgradeslots because UnsubOnLevelUp resets this event
    public event Action<int> OnLevelUp;

    public int MaxLevel => maxLevel;
    public int CurrentLevel { get; private set; }

    [SerializeField] private bool dissolveWhenMaxed = true;
    public bool DissolveWhenMaxed => dissolveWhenMaxed;

    [SerializeField] private int maxLevel;
    [SerializeField] private PlayerStatModifier[] statModifiersPerLevel;

    public override string Description {
        get {
            string statsStr = "";
            foreach (PlayerStatModifier modifier in statModifiersPerLevel) {
                statsStr += StatsFormatter.Instance.GetStatModifierStr(modifier) + "\n";

                string currentValue = StatsFormatter.Instance.GetCustomStatValueStr(modifier.PlayerStatType, CurrentLevel * modifier.Value);
                string maxValue = StatsFormatter.Instance.GetCustomStatValueStr(modifier.PlayerStatType, maxLevel * modifier.Value);
                statsStr += $"({currentValue}/{maxValue})\n";
            }

            return statsStr;
        }
    }

    public PersistentUpgradeType UpgradeType { get; private set; }

    public override void TryPlay(Vector2 position) {
        base.TryPlay(position);
        Play(position);
    }

    // upgrade card

    /// <summary>
    /// only add stat modifier if not max level
    /// if upgraded to max level and dissolveWhenMaxed, then play MaxPersisent sfx
    /// if 
    /// </summary>
    protected override void Play(Vector2 position) {
        base.Play(position);

        if (CurrentLevel == maxLevel) {
            UpgradeType = PersistentUpgradeType.AlreadyMaxed;
        }
        else if (CurrentLevel + 1 == maxLevel) {
            if (DissolveWhenMaxed) {
                UpgradeType = PersistentUpgradeType.Dissolve;
            }
            else {
                UpgradeType = PersistentUpgradeType.BecomingMaxed;
            }
        }
        else {
            UpgradeType = PersistentUpgradeType.NormalUpgrade;
        }


        if (UpgradeType != PersistentUpgradeType.AlreadyMaxed) {
            CurrentLevel++;
            StatsManager.AddPlayerStatModifiers(statModifiersPerLevel);
            OnLevelUp?.Invoke(CurrentLevel);
        }

        switch (UpgradeType) {
            case PersistentUpgradeType.NormalUpgrade:
                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.UpgradePersisent);
                break;
            case PersistentUpgradeType.Dissolve:
                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.MaxPersisent);
                break;
            case PersistentUpgradeType.BecomingMaxed:
                if (DissolveWhenMaxed) {
                    Debug.LogError("Should not already be maxed because should dissolve!");
                    return;
                }

                AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.UpgradePersisent);
                Cost = 0;
                break;
            case PersistentUpgradeType.AlreadyMaxed:
                if (DissolveWhenMaxed) {
                    Debug.LogError("Should not already be maxed because should have dissolved!");
                    return;
                }
                break;
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
    }

    public void UnsubOnLevelUp() {
        OnLevelUp = null;
    }
}

public enum PersistentUpgradeType { NormalUpgrade, Dissolve, BecomingMaxed, AlreadyMaxed }
