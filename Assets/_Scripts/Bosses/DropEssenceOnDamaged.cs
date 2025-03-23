using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEssenceOnDamaged : MonoBehaviour {

    private EnemyHealth health;

    private void Awake() {
        health = GetComponent<EnemyHealth>();

        health.OnDamaged_Damage_Shared += DropEssence;

    }

    private void OnDestroy() {
        health.OnDamaged_Damage_Shared -= DropEssence;
    }

    [SerializeField] private EssenceDrop essencePrefab;

    [SerializeField] private RandomInt dropAmount;
    [SerializeField][Range(0f, 1f)] private float dropChancePerDmg;

    [SerializeField] private float yVariation = 1f;

    private void DropEssence(float damage, bool shared) {

        float dropChance = dropChancePerDmg * damage;
        dropChance = Mathf.Clamp01(dropChance);

        if (Random.value < dropChance) {
            int amount = dropAmount.Randomize();
            for (int i = 0; i < amount; i++) {

                float yOffset = Random.Range(-yVariation, yVariation);
                Vector3 essencePos = transform.position + Vector3.up * yOffset;
                EssenceDrop essence = essencePrefab.Spawn(essencePos, Containers.Instance.Drops);

                essence.Launch();
            }
        }
    }
}
