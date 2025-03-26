using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EssenceHarvestCard", menuName = "Cards/Essence Harvest Card")]
public class ScriptableEssenceHarvestCard : ScriptablePersistentCard {

    public static float TotalDropMult;
    [SerializeField] private float dropMultPerLevel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Init() {
        TotalDropMult = 1;
    }

    protected override void Play(Vector2 position) {
        if (CurrentLevel < MaxLevel) {
            TotalDropMult += dropMultPerLevel;
            Debug.Log("Upgraded: " + TotalDropMult);
        }

        base.Play(position);
    }

    public override void OnRemoved() {
        base.OnRemoved();
        TotalDropMult -= dropMultPerLevel * CurrentLevel;
        Debug.Log("Removed: " + TotalDropMult);
    }
}
