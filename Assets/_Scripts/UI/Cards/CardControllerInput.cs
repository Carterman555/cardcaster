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
    private PlayFeedbackOnHover showOnHover;

    private bool movingCard; // true if able to move card with joystick
    private bool showing; // true if card is fully showing from pressing play action

    private void Awake() {
        handCard = GetComponent<HandCard>();
        showOnHover = GetComponent<PlayFeedbackOnHover>();
    }

    private void OnEnable() {
        showOnHover.enabled = false;
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

        if (!handCard.CanAffordToPlay()) {
            if (playInputPressed) {
                handCard.CantPlayShake();
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
                handCard.PlayCard(worldPos);
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
