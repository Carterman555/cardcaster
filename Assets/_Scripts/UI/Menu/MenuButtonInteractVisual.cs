using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInteractVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    [SerializeField] private Transform interactVisual;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool underlineOnHover = true;

    public void OnPointerEnter(PointerEventData eventData) {
        if (underlineOnHover) {
            ShowUnderline();
        }
    }
    public void OnPointerExit(PointerEventData eventData) {
        if (underlineOnHover) {
            HideUnderline();
        }
    }

    public void OnSelect(BaseEventData eventData) {
        if (underlineOnHover) {
            ShowUnderline();
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        if (underlineOnHover) {
            HideUnderline();
        }
    }

    public void ShowUnderline() {
        TMP_TextInfo textInfo = text.textInfo;

        // Force an update to ensure text metrics are current
        text.ForceMeshUpdate();

        if (textInfo.characterCount > 0) {
            float minX = textInfo.characterInfo[0].bottomLeft.x;
            float maxX = textInfo.characterInfo[textInfo.characterCount - 1].topRight.x;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            float leftX = rectTransform.position.x + minX;
            interactVisual.transform.position = new(leftX, interactVisual.transform.position.y);

            float length = maxX - minX;
            interactVisual.DOScaleX(length, duration: 0.1f).SetUpdate(true).SetEase(Ease.InFlash);
        }
    }

    public void HideUnderline() {
        interactVisual.DOScaleX(0, duration: 0.1f).SetUpdate(true).SetEase(Ease.OutFlash);
    }
}
