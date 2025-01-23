using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void OnEnable() {
        button.onClick.AddListener(PlayClickSFX);
    }
    private void OnDisable() {
        button.onClick.RemoveListener(PlayClickSFX);
    }

    [SerializeField] private bool playClickSFX;
    [ConditionalHide("playClickSFX")] [SerializeField] private bool customClickSFX;
    [ConditionalHide("customClickSFX")] [SerializeField] private AudioClips clickSFX;

    private void PlayClickSFX() {

        if (!playClickSFX) {
            return;
        }

        if (customClickSFX) {
            AudioManager.Instance.PlaySound(clickSFX, uiSound: true);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ButtonClick, uiSound: true);
        }
    }

    [SerializeField] private bool customHoverSFX;
    [ConditionalHide("customHoverSFX")][SerializeField] private AudioClips enterSFX;
    [ConditionalHide("customHoverSFX")][SerializeField] private AudioClips exitSFX;

    public void OnPointerEnter(PointerEventData eventData) {

        CanvasGroup canvasGroup = GetComponentInParent<CanvasGroup>();
        bool groupInteractable = canvasGroup == null || canvasGroup.interactable;

        if (!button.interactable || !groupInteractable) {
            return;
        }

        if (customHoverSFX) {
            AudioManager.Instance.PlaySound(enterSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ButtonEnter);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        CanvasGroup canvasGroup = GetComponentInParent<CanvasGroup>();
        bool groupInteractable = canvasGroup == null || canvasGroup.interactable;

        if (!button.interactable || !groupInteractable) {
            return;
        }

        if (customHoverSFX) {
            AudioManager.Instance.PlaySound(exitSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ButtonExit);
        }
    }
}
