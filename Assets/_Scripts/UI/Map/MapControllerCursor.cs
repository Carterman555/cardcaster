using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapControllerCursor : MonoBehaviour {

    [SerializeField] private InputActionReference cursorMoveAction;

    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private RectTransform panelTransform;

    [SerializeField] private RectTransform mapViewport;

    [SerializeField] private float cursorSpeed = 750f;

    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private InputActionReference clickInput;

    private Button buttonHovering;

    private void OnEnable() {
        InputManager.OnControlSchemeChanged += UpdateCursorActive;
        clickInput.action.performed += OnClick;

        UpdateCursorActive();
    }

    private void OnDisable() {
        InputManager.OnControlSchemeChanged -= UpdateCursorActive;
        clickInput.action.performed -= OnClick;
    }

    private void UpdateCursorActive() {
        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            cursorTransform.gameObject.SetActive(true);
        }
        else {
            cursorTransform.gameObject.SetActive(false);
        }
    }

    private void Update() {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Controller) {
            return;
        }

        HandleCurserMovement();
        HandleCurserSelect();
    }

    private void HandleCurserMovement() {
        Vector2 cursorMoveDirection = cursorMoveAction.action.ReadValue<Vector2>().normalized;

        Vector2 desiredCursorPos = cursorTransform.anchoredPosition + (cursorMoveDirection * cursorSpeed * Time.unscaledDeltaTime);

        Vector2 mapViewSize = mapViewport.rect.size;
        Vector2 cursorSize = cursorTransform.rect.size;

        // too far left
        float leftThreshold = -(mapViewSize.x / 2f) + (cursorSize.x / 2f);
        if (desiredCursorPos.x < leftThreshold) {
            desiredCursorPos.x = leftThreshold;
            panelTransform.anchoredPosition -= Vector2.left * cursorSpeed * Time.unscaledDeltaTime;
        }

        // too far right
        float rightThreshold = (mapViewSize.x / 2f) - (cursorSize.x / 2f);
        if (desiredCursorPos.x > rightThreshold) {
            desiredCursorPos.x = rightThreshold;
            panelTransform.anchoredPosition -= Vector2.right * cursorSpeed * Time.unscaledDeltaTime;
        }

        // too far up
        float topThreshold = (mapViewSize.y / 2f) - (cursorSize.y / 2f);
        if (desiredCursorPos.y > topThreshold) {
            desiredCursorPos.y = topThreshold;
            panelTransform.anchoredPosition -= Vector2.up * cursorSpeed * Time.unscaledDeltaTime;
        }

        // too far down
        float downThreshold = -(mapViewSize.y / 2f) + (cursorSize.y / 2f);
        if (desiredCursorPos.y < downThreshold) {
            desiredCursorPos.y = downThreshold;
            panelTransform.anchoredPosition -= Vector2.down * cursorSpeed * Time.unscaledDeltaTime;
        }

        cursorTransform.anchoredPosition = desiredCursorPos;
    }

    private void HandleCurserSelect() {
        GraphicRaycaster raycaster = popupCanvas.GetComponent<GraphicRaycaster>();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = cursorTransform.transform.position;

        List<RaycastResult> results = new();
        raycaster.Raycast(pointerData, results);

        Button newButtonHovering = null;

        foreach (RaycastResult result in results) {
            bool hasButton = result.gameObject.TryGetComponent(out Button button);
            bool hasTeleportButton = result.gameObject.TryGetComponent(out RoomTeleportButton roomTeleportButton);
            if (hasButton && hasTeleportButton && button.enabled) {
                newButtonHovering = button;
                break;
            }
        }

        bool buttonHoveringChanged = buttonHovering != newButtonHovering;
        if (buttonHoveringChanged) {
            if (newButtonHovering != null) {
                ExecuteEvents.Execute(newButtonHovering.gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
            }

            if (buttonHovering != null) {
                ExecuteEvents.Execute(buttonHovering.gameObject, pointerData, ExecuteEvents.pointerExitHandler);
            }

            buttonHovering = newButtonHovering;
        }
    }

    private void OnClick(InputAction.CallbackContext context) {
        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Controller) {
            return;
        }

        buttonHovering.onClick.Invoke();
    }
}
