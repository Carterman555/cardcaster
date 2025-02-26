using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayFeedbackOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private MMF_Player hoverFeedback;

    [SerializeField] private bool playSFX;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnEnterClips;
    [ConditionalHide("playSFX")][SerializeField] private AudioClips OnExitClips;

    private bool shouldBeInFirstState;
    private bool waitingToExecute;
    private float executeTimer;

    private HandCard card;

    private void OnEnable() {
        shouldBeInFirstState = true;
        executeTimer = 0f;

        card = GetComponent<HandCard>();
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        print($"{card.GetCard().CardType}: OnPointEnter");

        DelayedExecute(false);

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips, uiSound: true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        print($"{card.GetCard().CardType}: OnPointExit");

        DelayedExecute(true);

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips);
        }
    }

    /// <summary>
    /// this method will execute the command instantly if not executed within delay prior to execution. If within delay of prior execution,
    /// wait for delay timer to finish, then execute. If invoked while waiting for delay timer to finish, replace the previous command with 
    /// the newest one (and still wait to execute).
    /// </summary>
    /// 
    private void DelayedExecute(bool goToFirstState) {

        shouldBeInFirstState = goToFirstState;

        if (!waitingToExecute) {

            executeTimer = 0;

            if (goToFirstState) {
                if (hoverFeedback.IsPlaying) {

                    // if should be going to first, but going to second state, then revert
                    if (hoverFeedback.Direction == MMFeedbacks.Directions.TopToBottom) {
                        hoverFeedback.Revert();
                    }
                }
                else {

                    // if not playing, play to go to first state
                    hoverFeedback.SetDirectionBottomToTop();
                    hoverFeedback.PlayFeedbacks();
                }
            }
            else {
                if (hoverFeedback.IsPlaying) {

                    // if should be going to second, but going to first state, then revert
                    if (hoverFeedback.Direction == MMFeedbacks.Directions.BottomToTop) {
                        hoverFeedback.Revert();
                    }
                }
                else {

                    // if not playing, play to go to second state
                    hoverFeedback.SetDirectionTopToBottom();
                    hoverFeedback.PlayFeedbacks();
                }
            }
        }
    }

    private void Update() {

        if (waitingToExecute) {
            executeTimer += Time.deltaTime;
            float executeDelay = 0.1f;
            if (executeTimer > executeDelay) {
                executeTimer = float.NegativeInfinity;
                DelayedExecute(shouldBeInFirstState);
                waitingToExecute = false;
            }
        }
    }
}

