using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private Image healthFill;

    private EnemyHealth bossHealth;

    public void Setup(string bossName, EnemyHealth bossHealth) {
        this.bossHealth = bossHealth;

        bossNameText.text = bossName;

        healthFill.fillAmount = 1f;
        bossHealth.OnHealthChanged_HealthProportion += UpdateHealthBar;
    }
    private void OnDisable() {
        bossHealth.OnHealthChanged_HealthProportion -= UpdateHealthBar;
    }

    private void UpdateHealthBar(float healthProportion) {
        healthFill.fillAmount = healthProportion;
    }
}
