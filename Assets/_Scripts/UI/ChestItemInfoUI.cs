using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemInfoUI : MonoBehaviour, IInitializable {

    #region Static Instance

    public static ChestItemInfoUI Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    [SerializeField] private CardImage cardImage;
    [SerializeField] private GameObject heal;

    public void SetupCard(ScriptableCardBase card) {
        FeedbackPlayer.Play("ChestItemInfoPopup");

        cardImage.gameObject.SetActive(true);
        heal.SetActive(false);

        cardImage.Setup(card);
    }

    public void SetupHeal() {
        FeedbackPlayer.Play("ChestItemInfoPopup");

        cardImage.gameObject.SetActive(false);
        heal.SetActive(true);
    }
}
