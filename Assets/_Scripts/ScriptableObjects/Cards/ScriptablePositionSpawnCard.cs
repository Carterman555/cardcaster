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


        // if something is spawned and it doesn't have a duration, it needs to invoke Stop() to remove this ability
        // from being active in abilityManager
        if (!AbilityAttributes.HasFlag(AbilityAttribute.HasDuration)) {
            base.Stop();
        }
    }

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        abilityEffectPrefabs.Add(effectPrefab);
    }

    // applies the effects set by the modifier
    private void ApplyEffects(GameObject spawnedObject) {
        foreach (var abilityEffectPrefab in abilityEffectPrefabs) {
            abilityEffectPrefab.Spawn(spawnedObject.transform);
        }
    }
}
