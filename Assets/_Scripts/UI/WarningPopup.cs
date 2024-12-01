using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WarningPopup : MonoBehaviour, IInitializable {

    #region Static Instance

    public static WarningPopup Instance { get; private set; }

    public void Initialize() {
        Instance = this;
    }

    protected virtual void OnApplicationQuit() {
        Instance = null;
        Destroy(gameObject);
    }

    #endregion

    public event Action OnAccepted;

    [SerializeField] private TextMeshProUGUI warningText;

    private CanvasGroup canvasGroupToDisable;

    public void Setup(string warningTextStr, CanvasGroup canvasGroupToDisable) {
        warningText.text = warningTextStr;
        this.canvasGroupToDisable = canvasGroupToDisable;

        OpenWarning();
    }

    // invoked by yes button
    public void OnAcceptButtonClicked() {
        OnAccepted?.Invoke();
        CloseWarning();
    }

    private void OpenWarning() {
        gameObject.SetActive(true);

        canvasGroupToDisable.interactable = false;

        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration: 0.3f).SetUpdate(true);
    }

    public void CloseWarning() {

        transform.localScale = Vector3.one;
        transform.DOScale(0, duration: 0.3f).SetUpdate(true).OnComplete(() => {
            gameObject.SetActive(false);
            canvasGroupToDisable.interactable = true;
        });
    }
}
