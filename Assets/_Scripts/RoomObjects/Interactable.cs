using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour {

    public event Action OnInteract;

    [SerializeField] private InputActionReference interactAction;
    private bool withinRange;

    [Header("Text")]
    [SerializeField] private TextMeshPro interactableTextPrefab;
    private TextMeshPro interactableText;
    [SerializeField] private Vector2 textPosition;

    [Header("Outline")]
    private Material originalMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material outlineMaterial;

    private void Awake() {
        originalMaterial = spriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out InteractTrigger interactTrigger)) {

            spriteRenderer.material = outlineMaterial;

            interactableText = interactableTextPrefab.Spawn((Vector2)transform.position + textPosition, transform);

            // grow text
            transform.DOKill();
            interactableText.transform.localScale = Vector3.zero;
            interactableText.transform.DOScale(1, duration: 0.3f);

            withinRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.TryGetComponent(out InteractTrigger interactTrigger)) {
            spriteRenderer.material = originalMaterial;

            // shrink text
            interactableText.transform.DOKill();
            interactableText.transform.ShrinkThenDestroy();

            withinRange = false;
        }
    }

    private void Update() {
        if (withinRange && interactAction.action.triggered) {
            OnInteract?.Invoke();
        }
    }

    private void OnDisable() {
        spriteRenderer.material = originalMaterial;

        // shrink text
        if (interactableText != null) {
            interactableText.transform.DOKill();
            interactableText.transform.ShrinkThenDestroy();
        }
    }

    public bool IsWithinRange() {
        return withinRange;
    }

}
