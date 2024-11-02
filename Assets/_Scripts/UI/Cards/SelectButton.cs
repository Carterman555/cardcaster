using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SelectButton : GameButton, IInitializable {
    
    #region Static Instance

    public static SelectButton Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    public static event Action<PanelCardButton> OnSelect_PanelCard;

    [SerializeField] private TextMeshProUGUI text;

    private PanelCardButton panelCard;
    private CardLocation cardLocation;
    private int cardIndex;

    public void Show(string buttonText, Vector2 position, PanelCardButton panelCard) {
        text.text = buttonText;

        this.panelCard = panelCard;
        cardLocation = panelCard.GetCardLocation();
        cardIndex = panelCard.GetCardIndex();

        gameObject.SetActive(true);

        //... parent it to card so it moves with scroll
        transform.SetParent(panelCard.transform, false);
        transform.position = position;

        button.interactable = true;

        // grow
        float duration = 0.3f;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration).SetEase(Ease.OutSine).SetUpdate(true);
    }

    public void Hide() {

        // shrink then disable
        float duration = 0.3f;
        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.zero, duration).SetEase(Ease.InSine).SetUpdate(true).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    protected override void OnClick() {
        base.OnClick();

        OnSelect_PanelCard?.Invoke(panelCard);

        button.interactable = false;
        Hide();
    }

    
}
