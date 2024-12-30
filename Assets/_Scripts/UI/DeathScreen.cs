using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreen : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI levelText;

    private void OnEnable() {
        killsText.text = "Kills: " + GameStatsTracker.Instance.GetKills();
        levelText.text = "Level: " + GameSceneManager.Instance.GetLevel();
    }
}
