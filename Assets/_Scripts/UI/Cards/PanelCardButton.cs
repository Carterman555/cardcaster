using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCardButton : GameButton {

    [SerializeField] private CardImage cardImage;
    [SerializeField] private MMF_Player burnCardFeedbacks;

    [Header("Burn")]
    [SerializeField] private Image[] burnImages;
    [SerializeField] private Material burnMaterial;
    [SerializeField] private float fadeSpeed;
    private Material burnMaterialInstance;

    private CardLocation cardLocation;
    private int cardIndex;

    public void Setup(ScriptableCardBase card, CardLocation cardLocation, int cardIndex) {
        cardImage.Setup(card);

        this.cardLocation = cardLocation;
        this.cardIndex = cardIndex;

        burnCardFeedbacks.RestoreInitialValues();

        burnMaterialInstance = new Material(burnMaterial);
        for (int i = 0; i < burnImages.Length; i++) {
            Image image = burnImages[i];
            burnMaterialInstance.name = "burn " + i;
            image.material = burnMaterialInstance;
        }
    }

    protected override void OnClick() {
        base.OnClick();

        Vector2 offset = new Vector2(0f, 85f);
        TrashButton.Instance.Show((Vector2)transform.position + offset, this, cardLocation, cardIndex);
    }

    public IEnumerator TrashCardBurn() {

        burnCardFeedbacks.PlayFeedbacks();

        float fadeAmount = -0.1f;
        while (fadeAmount < 1f) {
            burnMaterialInstance.SetFloat("_FadeAmount", fadeAmount);
            fadeAmount += fadeSpeed * Time.unscaledDeltaTime; // unscaled so can play when timescale = 0
            yield return null;
        }

        PopupPlayer.Close("AllCards");
    }
}
