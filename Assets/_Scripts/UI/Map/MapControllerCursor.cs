using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapControllerCursor : MonoBehaviour {

    [SerializeField] private InputActionReference cursorMoveAction;

    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private RectTransform panelTransform;

    [SerializeField] private float cursorSpeed = 750f;

    private void OnEnable() {
        InputManager.OnControlSchemeChanged += OnControlSchemeChanged;
    }

    private void OnDisable() {
        InputManager.OnControlSchemeChanged -= OnControlSchemeChanged;
    }

    private void OnControlSchemeChanged() {
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

        Vector2 cursorMoveDirection = cursorMoveAction.action.ReadValue<Vector2>().normalized;
        cursorTransform.anchoredPosition += cursorMoveDirection * cursorSpeed * Time.unscaledDeltaTime;
    }
}
