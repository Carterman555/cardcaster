using UnityEngine;
using UnityEngine.UI;

public class EssenceUIManager : MonoBehaviour {

    [SerializeField] private Image[] essenceFillImages;

    private void OnEnable() {
        DeckManager.OnEssenceChanged_Amount += UpdateEssenceIcons;
    }
    private void OnDisable() {
        DeckManager.OnEssenceChanged_Amount -= UpdateEssenceIcons;
    }

    private void UpdateEssenceIcons(float essence) {
        
        print("Update essence: " + essence);

        int fullEssence = (int)essence;

        for (int i = 0; i < fullEssence; i++) {
            essenceFillImages[i].fillAmount = 1;
        }

        if (fullEssence < essenceFillImages.Length) {
            float remainingEssence = essence - fullEssence;
            essenceFillImages[fullEssence].fillAmount = remainingEssence;
        }

        for (int i = fullEssence + 1; i < essenceFillImages.Length; i++) {
            essenceFillImages[i].fillAmount = 0;
        }
    }
}
