using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour, IInitializable {

    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI levelText;

    // so subscribes even when object is not active
    public void Initialize() {
        Health.OnAnyDeath -= TrySetup;
        Health.OnAnyDeath += TrySetup;
    }

    // setup death screen when player dies
    private void TrySetup(Health health) {
        if (health.TryGetComponent(out PlayerMeleeAttack playerMeleeAttack)) {
            Setup();
        }
    }

    private void Setup() {
        Health.OnAnyDeath -= TrySetup;

        FeedbackPlayer.Play("DeathScreen");

        killsText.text = "Kills: " + GameStatsTracker.Instance.GetKills();
        levelText.text = "Level: " + GameSceneManager.Instance.GetLevel();
    }
}
