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

    private void UpdateEssenceIcons(int essence) {
        for (int i = 0; i < essence; i++) {
            essenceFillImages[i].fillAmount = 1;
        }

        for (int i = essence; i < essenceFillImages.Length; i++) {
            essenceFillImages[i].fillAmount = 0;
        }
    }
}
