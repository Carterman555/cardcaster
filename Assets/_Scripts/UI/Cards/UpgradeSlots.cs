using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlots : MonoBehaviour {

    private Image[] upgradeSlots;

    [SerializeField] private Sprite unupgradedSprite;
    [SerializeField] private Sprite upgradedSprite;

    public void Setup(ScriptablePersistentCard card) {

        card.OnLevelUp += LevelUp;

        upgradeSlots = GetComponentsInChildren<Image>();
        for (int i = 0; i < upgradeSlots.Length; i++) {
            if (i < card.MaxLevel) {
                upgradeSlots[i].gameObject.SetActive(true);
            }
            else {
                upgradeSlots[i].gameObject.SetActive(false);
            }

            if (i < card.CurrentLevel) {
                upgradeSlots[i].sprite = upgradedSprite;
            }
            else {
                upgradeSlots[i].sprite = unupgradedSprite;
            }

        }
    }

    private void LevelUp(int level) {
        upgradeSlots[level - 1].sprite = upgradedSprite;
    }
}
