using UnityEngine;

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

    public void Setup(ScriptableCardBase card) {
        cardImage.Setup(card);
    }
}
