using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class NewCardUnlockedPanel : MonoBehaviour, IInitializable {

    #region Static Instance

    public static NewCardUnlockedPanel Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    private void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [SerializeField] private CardImage cardImage;
    [SerializeField] private TextMeshProUGUI explanationText;

    public void Setup(ScriptableCardBase card) {
        cardImage.Setup(card);

        if (card.HasUnlockText) {
            explanationText.text = card.UnlockText;
        }
        else {
            // reset to original text
            explanationText.GetComponent<LocalizeStringEvent>().RefreshString();
        }
    }

    [ContextMenu("Refresh")]
    private void RefreshString() {
        explanationText.GetComponent<LocalizeStringEvent>().RefreshString();
    }
}
