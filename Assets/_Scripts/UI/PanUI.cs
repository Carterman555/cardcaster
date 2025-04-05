using UnityEngine;
using UnityEngine.InputSystem;

public class PanUI : MonoBehaviour {

    [SerializeField] private InputActionReference panAction;
    [SerializeField] private RectTransform targetTransform;

    [SerializeField] private float panSpeed = 750f;

    [SerializeField] private bool horizontal = true;
    [SerializeField] private bool vertical = true;

    private void Update() {
        Vector2 panDirection = panAction.action.ReadValue<Vector2>().normalized;

        if (!horizontal) panDirection.x = 0;
        if (!vertical) panDirection.y = 0;

        targetTransform.anchoredPosition -= panDirection * panSpeed * Time.unscaledDeltaTime;
    }
}
