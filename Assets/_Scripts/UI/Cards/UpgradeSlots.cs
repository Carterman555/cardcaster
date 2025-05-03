using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlots : MonoBehaviour {

    [SerializeField] private Image[] upgradeSlots;

    [SerializeField] private Sprite unupgradedSprite;
    [SerializeField] private Sprite upgradedSprite;

    [SerializeField] private bool isHandCard;

    private Image upgradeSlotToFill;

    public void Setup(ScriptablePersistentCard card) {
        if (isHandCard) {
            card.UnsubOnLevelUp();
            card.OnLevelUp += OnLevelUp;
        }

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

        upgradeSlotToFill = null;
    }

    // seperate when the card levels up and when the upgrade slot image is updated for polish
    private void OnLevelUp(int level) {
        if (upgradeSlotToFill != null) {
            Debug.LogError("On level up persistent, but upgradeSlotToFill is not null! FillInUpgradeSlot() was probably " +
                "not played when it should've been");
            return;
        }

        upgradeSlotToFill = upgradeSlots[level - 1];
    }

    // played by PersistentUpgradePlayer
    public void TryFillInUpgradeSlot() {
        if (upgradeSlotToFill != null) {
            upgradeSlotToFill.sprite = upgradedSprite;
        }
    }
}
