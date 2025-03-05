using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class CardControllerInput : MonoBehaviour {

    [SerializeField] private float cardMoveSpeed;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;

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
            print(handCard.GetCardState());

            if (handCard.GetCardState() == HandCard.CardState.ReadyToPlay) {

                bool positionalCard = handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional;
                if (positionalCard) {
                    StartMovingCard();
                }
                else {
                    showCardMovement.MoveUp();
                }

                handCard.OnStartPlayingCard();
            }
            else if (handCard.GetCardState() == HandCard.CardState.Playing) {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
                handCard.TryPlayCard(worldPos);
            }
        }

        bool pressedCancelInput = cancelAction.action.WasReleasedThisFrame();
        if (pressedCancelInput || PressedOtherPlayInput()) {
            if (handCard.GetCardState() == HandCard.CardState.Playing) {
                handCard.CancelCard(movingCard);
            }
            movingCard = false;
        }

        if (handCard.GetCardState() == HandCard.CardState.Playing && movingCard) {
            Vector3 direction = moveCardAction.action.ReadValue<Vector2>().normalized;
            transform.position += direction * cardMoveSpeed * Time.deltaTime;
        }
    }
    
    private void StartMovingCard() {
        MoveToCenter();

        if (handCard.GetCard() is ScriptableAbilityCardBase abilityCard) {
            abilityCard.OnStartPositioningCard(transform);
        }
    }

    private void MoveToCenter() {
        movingCard = true;
        handCard.SetCardState(HandCard.CardState.Moving);
        print($"{handCard.GetCard().CardType} - MoveToCenter: set state to moving");

        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        transform.DOMove(screenCenterPos, duration: 0.2f).OnComplete(() => {
            handCard.SetCardState(HandCard.CardState.Playing);
            print($"{handCard.GetCard().CardType} - MoveToCenter: set state to playing");
        });
    }

    private bool PressedOtherPlayInput() {
        int cardIndex = handCard.GetIndex();
        if (cardIndex == 0) {
            return playSecondCardInput.action.WasReleasedThisFrame() || playThirdCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 1) {
            return playFirstCardInput.action.WasReleasedThisFrame() || playThirdCardInput.action.WasReleasedThisFrame();
        }
        else if (cardIndex == 2) {
            return playFirstCardInput.action.WasReleasedThisFrame() || playSecondCardInput.action.WasReleasedThisFrame();
        }
        else {
            Debug.LogError("cardIndex not supported: " + cardIndex);
            return false;
        }
    }
}
