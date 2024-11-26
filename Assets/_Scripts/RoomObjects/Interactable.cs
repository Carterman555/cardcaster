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
    private bool canInteract;

    [Header("Text")]
    [SerializeField] private TextMeshPro interactableTextPrefab;
    private TextMeshPro interactableText;
    [SerializeField] private Vector2 textPosition;
    [SerializeField] private string text = "E";

    [Header("Outline")]
    private Material originalMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material outlineMaterial;

    private void Awake() {
        originalMaterial = spriteRenderer.material;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (Helpers.GameStopping()) {
            return;
        }

        if (collision.TryGetComponent(out InteractTrigger interactTrigger) && enabled) {
            InteractManager.Instance.AddWithinRange(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (Helpers.GameStopping()) {
            return;
        }

        if (collision.TryGetComponent(out InteractTrigger interactTrigger) && enabled) {
            InteractManager.Instance.RemoveWithinRange(this);
        }
    }

    public void SetCanInteract() {

        canInteract = true;

        spriteRenderer.material = outlineMaterial;

        interactableText = interactableTextPrefab.Spawn((Vector2)transform.position + textPosition, transform);

        interactableText.text = text;

        // grow text
        interactableText.transform.DOKill();
        interactableText.transform.localScale = Vector3.zero;
        interactableText.transform.DOScale(1, duration: 0.3f);
    }

    public void SetCantInteract() {

        canInteract = false;

        spriteRenderer.material = originalMaterial;

        // shrink text
        interactableText.transform.DOKill();
        interactableText.transform.ShrinkThenDestroy();
    }

    private void OnDisable() {

        if (Helpers.GameStopping()) {
            return;
        }

        InteractManager.Instance.TryRemoveWithinRange(this);

        spriteRenderer.material = originalMaterial;

        // shrink text
        bool interactableTextActive = interactableText != null && interactableText.enabled == true;
        if (interactableTextActive) {
            interactableText.transform.DOKill();
            interactableText.transform.ShrinkThenDestroy();
        }
    }

    private void Update() {
        if (canInteract && interactAction.action.triggered) {
            OnInteract?.Invoke();
        }
    }

    public bool CanInteract() {
        return canInteract;
    }
}
