using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Position Spawn")]
public class ScriptablePositionSpawnCard : ScriptableAbilityCardBase {

    [SerializeField] private GameObject objectToSpawn;

    protected override void Play(Vector2 position) {
        base.Play(position);

        GameObject newObject = objectToSpawn.Spawn(position, Containers.Instance.Projectiles);

        if (newObject.TryGetComponent(out IAbilityStatsSetup abilityStatsSetup)) {
            abilityStatsSetup.SetAbilityStats(Stats);
        }

        SpawnAbilityEffects(newObject.transform);

        // if something is spawned and it doesn't have a duration, it needs to invoke Stop() because normally it
        // is invoked after the duration, but it won't be without a duration
        if (!AbilityAttributes.HasFlag(AbilityAttribute.HasDuration)) {
            Stop();
        }
    }

    private List<EffectModifier> effectModifiers = new();

    public override void ApplyModifier(ScriptableModifierCardBase modifierCard) {
        base.ApplyModifier(modifierCard);
        if (modifierCard.AppliesEffect) {
            effectModifiers.Add(modifierCard.EffectModifier);
        }
    }

    // the effects are returned by ReturnOnDisable
    private void SpawnAbilityEffects(Transform spawnedTransform) {
        foreach (EffectModifier effectModifier in effectModifiers) {
            effectModifier.EffectLogicPrefab.Spawn(spawnedTransform);

            if (effectModifier.HasVisual) {
                Transform visualTransform = spawnedTransform.Find("Visual");
                if (visualTransform == null) {
                    Debug.LogError($"Object {spawnedTransform.name} does not have child with name 'Visual'!");
                    return;
                }

                effectModifier.EffectVisualPrefab.Spawn(visualTransform);
            }
        }
        effectModifiers.Clear();
    }
}
