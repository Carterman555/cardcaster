using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.UI;

public class EssenceUIManager : StaticInstance<EssenceUIManager> {

    private List<Transform> essenceIcons;

    [SerializeField] private Transform essenceIconPrefab;

    private void OnEnable() {
        DeckManager.OnEssenceChanged_Amount += UpdateEssenceSprites;
        DeckManager.OnMaxEssenceChanged_Amount += UpdateEssenceMaxAmount;

        essenceIcons = new();
        foreach (Transform essenceIcon in transform) {
            if (essenceIcon.gameObject.activeSelf) {
                essenceIcons.Add(essenceIcon);
            }
        }

        UpdateEssenceSprites(DeckManager.Instance.Essence);
        UpdateEssenceMaxAmount(StatsManager.PlayerStats.MaxEssence);
    }
    private void OnDisable() {
        DeckManager.OnEssenceChanged_Amount -= UpdateEssenceSprites;
        DeckManager.OnMaxEssenceChanged_Amount -= UpdateEssenceMaxAmount;
    }

    private void UpdateEssenceSprites(int amount) {
        for (int i = 0; i < amount; i++) {
            Image fillImage = essenceIcons[i].GetChild(1).GetComponent<Image>();
            fillImage.fillAmount = 1;
        }

        for (int i = amount; i < essenceIcons.Count; i++) {
            Image fillImage = essenceIcons[i].GetChild(1).GetComponent<Image>();
            fillImage.fillAmount = 0;
        }
    }

    private void UpdateEssenceMaxAmount(int maxEssence) {
        while (maxEssence > essenceIcons.Count) {
            Transform essenceIcon = essenceIconPrefab.Spawn(transform);
            essenceIcons.Add(essenceIcon);
        }

        while (maxEssence < essenceIcons.Count) {
            essenceIcons[^1].gameObject.ReturnToPool();
            essenceIcons.RemoveAt(essenceIcons.Count - 1);
        }

        UpdateEssenceSprites(DeckManager.Instance.Essence);
    }
}
