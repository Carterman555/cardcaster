using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEssenceOnDamaged : MonoBehaviour {

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();

        health.OnDamaged_Damage_Shared += DropEssence;
    }

    private void OnDestroy() {
        health.OnDamaged_Damage_Shared -= DropEssence;
    }

    [SerializeField] private GameObject essencePrefab;

    [SerializeField] private RandomInt dropAmount;
    [SerializeField][Range(0f, 1f)] private float dropChancePerDmg;

    private void DropEssence(float damage, bool shared) {

        float dropChance = dropChancePerDmg * damage;
        dropChance = Mathf.Clamp01(dropChance);

        if (Random.value < dropChance) {
            int amount = dropAmount.Randomize();
            for (int i = 0; i < amount; i++) {
                essencePrefab.Spawn(transform.position, Containers.Instance.Drops);
            }
        }
    }
}
