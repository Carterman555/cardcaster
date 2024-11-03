using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCardButton : GameButton {

    public static event Action<PanelCardButton> OnClicked_PanelCard;

    [SerializeField] private CardImage cardImage;

    private ScriptableCardBase card;
    private CardLocation cardLocation;
    private int cardIndex;

    public void Setup(ScriptableCardBase card, CardLocation cardLocation, int cardIndex) {
        this.card = card;
        this.cardLocation = cardLocation;
        this.cardIndex = cardIndex;


        cardImage.Setup(card);
        
        SetupTrashing();
    }

    protected override void OnEnable() {
        base.OnEnable();

        // the button can be set to not interactable by shopUIManager so make sure it's reset here
        GetComponent<Button>().interactable = true;
    }

    protected override void OnClick() {
        base.OnClick();
        OnClicked_PanelCard?.Invoke(this);
    }

    #region Trash

    [Header("Trashing")]
    [SerializeField] private MMF_Player burnCardFeedbacks;
    [SerializeField] private Image[] burnImages;
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float fadeSpeed;
    private Material burnMaterialInstance;

    private void SetupTrashing() {
        burnCardFeedbacks.RestoreInitialValues();

        burnMaterialInstance = new Material(burnMaterial);
        for (int i = 0; i < burnImages.Length; i++) {
            Image image = burnImages[i];
            burnMaterialInstance.name = "burn " + i;
            image.material = burnMaterialInstance;
        }
    }

    public void Trash() {
        DeckManager.Instance.TrashCard(cardLocation, cardIndex);
        StartCoroutine(TrashCardVisual());
    }

    public IEnumerator TrashCardVisual() {

        burnCardFeedbacks.PlayFeedbacks();

        float fadeAmount = -0.1f;
        while (fadeAmount < 1f) {
            burnMaterialInstance.SetFloat("_FadeAmount", fadeAmount);
            fadeAmount += fadeSpeed * Time.unscaledDeltaTime; // unscaled so can play when timescale = 0
            yield return null;
        }

        FeedbackPlayer.PlayInReverse("OpenAllCardsPanel");
    }

    #endregion

    public ScriptableCardBase GetCard() {
        return card;
    }

    public CardLocation GetCardLocation() {
        return cardLocation;
    }

    public int GetCardIndex() {
        return cardIndex;
    }
}
