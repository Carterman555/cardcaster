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

    [SerializeField] protected MMF_Player showCardPlayer;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference playFirstCardInput;
    [SerializeField] private InputActionReference playSecondCardInput;
    [SerializeField] private InputActionReference playThirdCardInput;
    private InputActionReference playInput;

    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private InputActionReference moveCardAction;

    private HandCard handCard;

    private bool movingCard; // true if able to move card with joystick
    private bool showing; // true if card is fully showing from pressing play action

    private void Awake() {
        handCard = GetComponent<HandCard>();
    }

    private void OnDisable() {
        showing = false;
        movingCard = false;
    }

    private void Update() {
        HandleInput();
    }

    private void HandleInput() {
        bool playInputPressed = GetPlayInput().WasReleasedThisFrame();

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
                    print("show card");
                    showing = true;
                    showCardPlayer.SetDirectionTopToBottom();
                    showCardPlayer.PlayFeedbacks();
                }
                else {
                    print("move card");
                    movingCard = true;
                }

                handCard.OnStartPlayingCard();
            }
            else if (showing || movingCard) {
                print("play card");
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(transform.position);
                handCard.PlayCard(worldPos);
            }
        }

        if (cancelAction.action.WasReleasedThisFrame()) {
            handCard.CancelCard();
        }

        if (movingCard) {
            Vector3 direction = moveCardAction.action.ReadValue<Vector2>().normalized;
            transform.position += direction * cardMoveSpeed * Time.deltaTime;
        }
    }

    private InputAction GetPlayInput() {
        int cardIndex = handCard.GetIndex();
        if (cardIndex == 0) {
            return playFirstCardInput.action;
        }
        else if (cardIndex == 1) {
            return playSecondCardInput.action;
        }
        else if (cardIndex == 2) {
            return playThirdCardInput.action;
        }
        else {
            Debug.LogError("cardIndex not supported: " + cardIndex);
            return null;
        }
    }
}
