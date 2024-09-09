using UnityEngine;

public class UIFollowMouse : StaticInstance<UIFollowMouse> {
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;

    protected override void Awake() {
        base.Awake();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    private void LateUpdate() {
        Vector2 mousePosition = Input.mousePosition;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
            rectTransform.position = mousePosition;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                mousePosition,
                canvas.worldCamera,
                out localPoint);

            rectTransform.localPosition = localPoint;
        }
    }
}