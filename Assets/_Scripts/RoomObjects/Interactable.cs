using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour {

    public event Action OnInteract;

    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private TextMeshPro interactableTextPrefab;

    private bool withinRange;

    private SpriteRenderer spriteRenderer;

    private Material originalMaterial;
    [SerializeField] private Material outlineMaterial;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalMaterial = spriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == GameLayers.PlayerLayer) {
            spriteRenderer.material = outlineMaterial;
            withinRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == GameLayers.PlayerLayer) {
            spriteRenderer.material = originalMaterial;
            withinRange = false;
        }
    }

    private void Update() {
        if (withinRange && interactAction.action.triggered) {
            OnInteract?.Invoke();
        }
    }

    public bool IsWithinRange() {
        return withinRange;
    }

}
