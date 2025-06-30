using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CompendiumSectionButton : GameButton, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    private static event Action OnSelected;

    private static CompendiumSectionButton selectedSectionButton;
    private bool Selected => selectedSectionButton == this;

    private MenuButtonInteractVisual menuButtonInteractVisual;

    [SerializeField] private GameObject menu;
    [SerializeField] private bool startSelected;
    
    protected override void Awake() {
        base.Awake();

        menuButtonInteractVisual = GetComponent<MenuButtonInteractVisual>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        OnSelected += UpdateSelected;

        //... wait for other buttons to subscribe
        DOVirtual.DelayedCall(Time.deltaTime, () => {
            if (startSelected) {
                menu.SetActive(true);

                selectedSectionButton = this;
                menuButtonInteractVisual.ShowUnderline();
                OnSelected?.Invoke();
            }
        });
    }

    protected override void OnDisable() {
        base.OnDisable();

        OnSelected -= UpdateSelected;
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        menuButtonInteractVisual.ShowUnderline();
    }

    public void OnSelect(BaseEventData eventData) {
        menuButtonInteractVisual.ShowUnderline();
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        if (!Selected) {
            menuButtonInteractVisual.HideUnderline();
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        if (!Selected) {
            menuButtonInteractVisual.HideUnderline();
        }
    }

    protected override void OnClick() {
        base.OnClick();

        menu.SetActive(true);

        selectedSectionButton = this;
        OnSelected?.Invoke();
    }

    private void UpdateSelected() {
        if (!Selected) {
            menu.SetActive(false);

            menuButtonInteractVisual.HideUnderline();
        }
    }
}
