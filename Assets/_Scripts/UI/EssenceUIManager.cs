using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EssenceUIManager : StaticInstance<EssenceUIManager> {

    private List<Image> essenceFillImages;

    [SerializeField] private Image essenceImagePrefab;

    private void OnEnable() {
        DeckManager.OnEssenceChanged_Amount += UpdateEssenceSprites;
        DeckManager.OnMaxEssenceChanged_Amount += UpdateEssenceAmount;

        UpdateEssenceImageList();
        
    }
    private void OnDisable() {
        DeckManager.OnEssenceChanged_Amount -= UpdateEssenceSprites;
        DeckManager.OnMaxEssenceChanged_Amount -= UpdateEssenceAmount;
    }

    private void UpdateEssenceSprites(int amount) {
        for (int i = 0; i < amount; i++) {
            essenceFillImages[i].fillAmount = 1;
        }

        for (int i = amount; i < essenceFillImages.Count; i++) {
            essenceFillImages[i].fillAmount = 0;
        }
    }

    private void UpdateEssenceAmount(int maxEssence) {



        UpdateEssenceImageList();
    }

    private void UpdateEssenceImageList() {
        essenceFillImages = new();
        foreach (Transform playerEssenceIcon in transform) { // will iterate over inactive children so keep that in mind
            Image fillImage = playerEssenceIcon.GetChild(1).GetComponent<Image>();
            essenceFillImages.Add(fillImage);
        }
    }
}
