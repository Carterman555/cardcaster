using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashButton : GameButton, IInitializable {
    
    #region Static Instance

    public static TrashButton Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    private PanelCardButton cardToTrash;
    private CardLocation cardLocation;
    private int cardIndex;

    public void Show(Vector2 position, PanelCardButton cardToTrash, CardLocation cardLocation, int cardIndex) {
        this.cardToTrash = cardToTrash;
        this.cardLocation = cardLocation;
        this.cardIndex = cardIndex;

        gameObject.SetActive(true);

        //... parent it to card so it moves with scroll
        transform.SetParent(cardToTrash.transform, false);
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

        button.interactable = false;
        DeckManager.Instance.TrashCard(cardLocation, cardIndex);
        DeckManager.Instance.StartCoroutine(cardToTrash.TrashCardBurn());

        Hide();
    }

    
}
