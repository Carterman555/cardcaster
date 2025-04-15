using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class Interactable : MonoBehaviour {

    public event Action<bool> OnChangeCanInteract;

    public event Action OnInteract;

    [SerializeField] private InputActionReference interactAction;
    public bool CanInteract { get; private set; }

    [Header("Text")]
    [SerializeField] private TextMeshPro interactableTextPrefab;
    private TextMeshPro interactableText;
    [SerializeField] private Vector2 textPosition;
    [SerializeField] private string text;
    [SerializeField] private LocalizedString locText;

    [Header("Outline")]
    private Material originalMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Material outlineMaterial;

    private void Awake() {
        originalMaterial = spriteRenderer.material;
    }

    private void OnEnable() {

        Collider2D[] touchingCols = new Collider2D[0];
        if (TryGetComponent(out BoxCollider2D boxCollider2D)) {
            touchingCols = Physics2D.OverlapBoxAll((Vector2)transform.position + boxCollider2D.offset, boxCollider2D.size, 0f);
        }
        else {
            Debug.LogWarning("Could not get collider component for interactable");
        }

        bool touchingInteractTrigger = touchingCols.Any(c => c.TryGetComponent(out InteractTrigger interactTrigger));
        if (touchingInteractTrigger) {
            InteractManager.Instance.TryAddWithinRange(this);
        }
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
        if (CanInteract && interactAction.action.triggered) {
            OnInteract?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (Helpers.GameStopping()) {
            return;
        }

        if (collision.TryGetComponent(out InteractTrigger interactTrigger) && enabled) {
            InteractManager.Instance.TryAddWithinRange(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (Helpers.GameStopping()) {
            return;
        }

        if (collision.TryGetComponent(out InteractTrigger interactTrigger) && enabled) {
            InteractManager.Instance.TryRemoveWithinRange(this);
        }
    }

    public void SetCanInteract() {

        CanInteract = true;

        spriteRenderer.material = outlineMaterial;

        interactableText = interactableTextPrefab.Spawn(transform);

        interactableText.transform.DOKill();
        transform.DOKill();

        //... for some reason, there is a bug when setting pos in spawn method
        interactableText.transform.position = (Vector2)transform.position + textPosition;

        string interactInputText = InputManager.Instance.GetBindingText(interactAction);
        interactableText.text = locText.GetLocalizedString() + " (" + interactInputText + ")";

        // grow text
        interactableText.transform.DOKill();
        interactableText.transform.localScale = Vector3.zero;
        interactableText.transform.DOScale(1, duration: 0.3f);

        OnChangeCanInteract?.Invoke(true);
    }

    public void SetCantInteract() {

        CanInteract = false;

        spriteRenderer.material = originalMaterial;

        // shrink text
        interactableText.transform.DOKill();
        interactableText.transform.ShrinkThenDestroy();

        OnChangeCanInteract?.Invoke(false);
    }
}
