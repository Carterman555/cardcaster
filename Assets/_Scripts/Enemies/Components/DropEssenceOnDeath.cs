using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropEssenceOnDeath : MonoBehaviour {

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();

        health.OnDeath += DropEssence;
    }

    private void OnDestroy() {
        health.OnDeath -= DropEssence;
    }

    [SerializeField] private GameObject essencePrefab;

    [SerializeField] private RandomInt dropAmount;
    [SerializeField][Range(0f, 1f)] private float dropChance;

    private void DropEssence() {

        if (UnityEngine.Random.value < dropChance) {
            int amount = dropAmount.Randomize();
            for (int i = 0; i < amount; i++) {
                essencePrefab.Spawn(transform.position, Containers.Instance.Drops);
            }
        }
    }

}
