using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetStatTexts : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI levelText;

    // for localized string event
    public int KillCount;
    public int Level;

    private void OnEnable() {
        killsText.text = "Kills: " + GameStatsTracker.Instance.GetKills();
        levelText.text = "Level: " + GameSceneManager.Instance.GetLevel();

        KillCount = GameStatsTracker.Instance.GetKills();
        Level = GameSceneManager.Instance.GetLevel();
    }
}
