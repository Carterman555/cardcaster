using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ClearDeckOnDeath : MonoBehaviour {

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        health.OnDeath += ClearDeck;
    }
    private void OnDisable() {
        health.OnDeath -= ClearDeck;
    }

    private void ClearDeck() {
        DeckManager.Instance.ResetDeckAndEssence();
    }
}
