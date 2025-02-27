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

    [SerializeField] private MMF_Player showCardPlayer;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;

    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private InputActionReference moveCardAction;

    private HandCard handCard;
    private ShowCardMovement showCardMovement;

    private bool movingCard; // true if able to move card with joystick
    private bool showing; // true if card is fully showing from pressing play action

    private void Awake() {
        handCard = GetComponent<HandCard>();
        showCardMovement = GetComponent<ShowCardMovement>();
    }

    private void OnEnable() {
        showCardMovement.enabled = false;
    }

    private void OnDisable() {
        showing = false;
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
            if (!showing && !movingCard) {

                bool positionalCard = handCard.GetCard() is ScriptableAbilityCardBase abilityCard && abilityCard.IsPositional;
                if (!positionalCard) {
                    showing = true;
                    showCardPlayer.SetDirectionTopToBottom();
                    showCardPlayer.PlayFeedbacks();
                }
                else {
                    StartMovingCard();
                }

                handCard.OnStartPlayingCard();
            }
            else if (showing || movingCard) {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
                handCard.TryPlayCard(worldPos);
            }
        }

        bool pressedCancelInput = cancelAction.action.WasReleasedThisFrame();
        if (pressedCancelInput || PressedOtherPlayInput()) {
            handCard.CancelCard();
            movingCard = false;
            showing = false;
        }

        if (movingCard) {
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
        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        transform.DOMove(screenCenterPos, duration: 0.2f).OnComplete(() => {
            movingCard = true; // allow player to move card after done moving to center
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
