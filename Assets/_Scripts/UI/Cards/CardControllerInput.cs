using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using static HandCard;

public class CardControllerInput : MonoBehaviour {

    [SerializeField] private float cardMoveSpeed;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;
    [SerializeField] private InputActionReference playForthCardInput;
    [SerializeField] private InputActionReference playFifthCardInput;
    [SerializeField] private InputActionReference playSixthCardInput;

    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private InputActionReference moveCardAction;

    private HandCard handCard;
    private ShowCardMovement showCardMovement;

    private bool movingCard; // true if able to move card with joystick

    private void Awake() {
        handCard = GetComponent<HandCard>();
        showCardMovement = GetComponent<ShowCardMovement>();
    }

    private void OnDisable() {
        if (handCard.CurrentCardState == CardState.Playing) {
            handCard.CancelCard(movingCard);
        }

        movingCard = false;
    }

    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        bool playInputPressed = handCard.GetPlayInput().WasReleasedThisFrame();

        if (!handCard.CanAffordToPlay() || !handCard.GetCard().CanPlay()) {
            if (playInputPressed) {
                handCard.CantPlayShake();

                // if tries to play a card that is incompatible with an active ability, show incompatible text
                if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsIncompatibleAbilityActive()) {
                    StartCoroutine(handCard.ShowIncompatibleText());
                }
            }
            return;
        }

        if (playInputPressed) {
            if (handCard.CurrentCardState == CardState.ReadyToPlay) {

                if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional) {
                    MoveToCenter();
                    abilityCard.OnStartPositioningCard(transform);

                    showCardMovement.OnPositioningCard();
                }
                else {
                    showCardMovement.Show();
                }

                handCard.OnStartPlayingCard();
            }
            else if (handCard.CurrentCardState == CardState.Playing) {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
                handCard.TryPlayCard(worldPos);
            }
        }

        bool pressedCancelInput = cancelAction.action.WasReleasedThisFrame();
        if (pressedCancelInput || PressedOtherPlayInput()) {
            if (handCard.CurrentCardState == CardState.Playing) {
                handCard.CancelCard(movingCard);
            }
            movingCard = false;
        }

        if (handCard.CurrentCardState == CardState.Playing && movingCard) {
            Vector3 direction = moveCardAction.action.ReadValue<Vector2>().normalized;

            // can't move off screen
            if (transform.position.x > Screen.width && direction.x > 0) {
                direction.x = 0;
            }
            if (transform.position.x < 0 && direction.x < 0) {
                direction.x = 0;
            }
            if (transform.position.y > Screen.height && direction.y > 0) {
                direction.y = 0;
            }
            if (transform.position.y < 0 && direction.y < 0) {
                direction.y = 0;
            }

            transform.position += direction * cardMoveSpeed * Time.deltaTime;
        }
    }

    private void MoveToCenter() {
        movingCard = true;
        handCard.CurrentCardState = CardState.Moving;

        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        transform.DOMove(screenCenterPos, duration: 0.2f).OnComplete(() => {
            handCard.CurrentCardState = CardState.Playing;
        });
    }

    private bool PressedOtherPlayInput() {
        int cardIndex = handCard.GetIndex();

        bool pressedAnyCardInput = playFirstCardInput.action.WasReleasedThisFrame() ||
            playSecondCardInput.action.WasReleasedThisFrame() ||
            playThirdCardInput.action.WasReleasedThisFrame() ||
            playForthCardInput.action.WasReleasedThisFrame() ||
            playFifthCardInput.action.WasReleasedThisFrame() ||
            playSixthCardInput.action.WasReleasedThisFrame();

        if (!pressedAnyCardInput) {
            return false;
        }

        if (cardIndex == 0) {
            return !playFirstCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 1) {
            return !playSecondCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 2) {
            return !playThirdCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 3) {
            return !playForthCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 4) {
            return !playFifthCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 5) {
            return !playSixthCardInput.action.WasReleasedThisFrame();
        }
        else {
            Debug.LogError("cardIndex not supported: " + cardIndex);
            return false;
        }
    }
}
