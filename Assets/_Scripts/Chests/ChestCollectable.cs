using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestCollectable : MonoBehaviour {

    [SerializeField] private InputActionReference selectAction;

    [SerializeField] private Vector2 positionOffset;

    private Material originalMaterial;
    [SerializeField] private Material outlineMaterial;

    private Chest chest;
    private ICollectable collectable;
    private int collectableIndex;

    private bool interactable;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Chest chest, ICollectable collectable, int collectableIndex) {
        this.chest = chest;
        this.collectable = collectable;
        this.collectableIndex = collectableIndex;

        spriteRenderer.sprite = collectable.GetSprite();
        originalMaterial = spriteRenderer.material;

        transform.position = chest.transform.position;
        transform.localScale = Vector2.zero;

        float duration = 0.3f;
        transform.DOLocalMove(positionOffset, duration).SetEase(Ease.OutSine);
        transform.DOScale(Vector2.one, duration).SetEase(Ease.OutSine).OnComplete(() => {
            interactable = true;
        });
    }

    private bool outlined;

    private void Update() {

        if (interactable && MouseTracker.Instance.IsMouseOver(gameObject)) {

            if (!outlined) {
                spriteRenderer.material = outlineMaterial;
                outlined = true;
            }

            //if (selectAction.action.triggered) {
            if (Input.GetMouseButtonDown(0)) {
                GoToPlayer();
                StartCoroutine(chest.OnSelectCollectable(collectableIndex));
            }
        }
        else {
            if (outlined) {
                //... disable outline
                spriteRenderer.material = originalMaterial;
                outlined = false;
            }
        }
    }

    public void GoToPlayer() {

        interactable = false;

        // so it doesn't disappear when the chest does
        transform.SetParent(Containers.Instance.Effects);

        float duration = 0.5f;
        transform.DOMove(PlayerMovement.Instance.transform.position, duration).SetEase(Ease.InSine).OnComplete(() => {
            transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine).OnComplete(() => {
                GainCollectable();
            });
        });
    }

    private void GainCollectable() {
        if (collectable is ScriptableCardBase scriptableCard) {
            DeckManager.Instance.GainCard(scriptableCard);
        }
    }

    public void ReturnToChest(float duration) {
        interactable = false;

        transform.DOLocalMove(Vector2.zero, duration).SetEase(Ease.InSine);
        transform.DOScale(Vector2.zero, duration).SetEase(Ease.InSine);
    }
}
