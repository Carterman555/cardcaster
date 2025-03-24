using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Position Spawn")]
public class ScriptablePositionSpawnCard : ScriptableAbilityCardBase {

    [SerializeField] private GameObject objectToSpawn;

    private List<GameObject> abilityEffectPrefabs = new();

    protected override void Play(Vector2 position) {
        base.Play(position);

        GameObject newObject = objectToSpawn.Spawn(position, Containers.Instance.Projectiles);

        if (newObject.TryGetComponent(out IAbilityStatsSetup abilityStatsSetup)) {
            abilityStatsSetup.SetAbilityStats(Stats);
        }

        ApplyEffects(newObject);

        // if something is spawned and it doesn't have a duration, it needs to invoke Stop() because normally it
        // is invoked after the duration, but it won't be without a duration
        if (!AbilityAttributes.HasFlag(AbilityAttribute.HasDuration)) {
            Stop();
        }
    }

    public override void Stop() {
        base.Stop();

        // just clear the list instead of returning the ability effects on stop because summons without a duration
        // play stop right after play. This would make returning the effects now not apply to those summons. The
        // effects are instead returned by ReturnOnDisable
        abilityEffectPrefabs.Clear();
    }

    public override void ApplyModifier(AbilityStats statsModifier, AbilityAttribute abilityAttributesToModify, GameObject effectPrefab) {
        base.ApplyModifier(statsModifier, abilityAttributesToModify, effectPrefab);
        if (effectPrefab != null) {
            abilityEffectPrefabs.Add(effectPrefab);
        }
    }

    // applies the effects set by the modifier
    private void ApplyEffects(GameObject spawnedObject) {
        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(spawnedObject.transform);
        }
    }
}
