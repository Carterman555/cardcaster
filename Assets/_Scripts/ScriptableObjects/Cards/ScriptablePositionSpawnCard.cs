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

        SpawnAbilityEffects(newObject);

        // if something is spawned and it doesn't have a duration, it needs to invoke Stop() because normally it
        // is invoked after the duration, but it won't be without a duration
        if (!AbilityAttributes.HasFlag(AbilityAttribute.HasDuration)) {
            Stop();
        }
    }

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            abilityEffectPrefabs.Add(modifierCard.EffectPrefab);
        }
    }

    // applies the effects set by the modifier
    // the effects are returned by ReturnOnDisable
    private void SpawnAbilityEffects(GameObject spawnedObject) {
        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(spawnedObject.transform);
        }
        abilityEffectPrefabs.Clear();
    }
}
