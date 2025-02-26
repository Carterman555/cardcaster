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

    private Queue<FeedbacksCommand> commands = new();

    public enum FeedbacksCommand { PlayNormal, PlayReversed, RevertToNormal, RevertToReversed }

    private HandCard card;

    private void OnEnable() {
        commands.Clear();

        card = GetComponent<HandCard>();

        StartCoroutine(ExecuteCommands());
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        print($"{card.GetCard().CardType}: OnPointEnter");

        if (hoverFeedback.IsPlaying) {
            commands.Enqueue(FeedbacksCommand.RevertToNormal);
            print($"{card.GetCard().CardType}: Queue Revert");
        }
        else {
            commands.Enqueue(FeedbacksCommand.PlayNormal);
            print($"{card.GetCard().CardType}: Queue Play Normal");
        }

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips, uiSound: true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (!enabled) {
            return;
        }

        print($"{card.GetCard().CardType}: OnPointExit");


        if (hoverFeedback.IsPlaying) {
            commands.Enqueue(FeedbacksCommand.RevertToReversed);
            print($"{card.GetCard().CardType}: Queue Revert");
        }
        else {
            commands.Enqueue(FeedbacksCommand.PlayReversed);
            print($"{card.GetCard().CardType}: Queue Play Reverse");
        }

        if (playSFX) {
            AudioManager.Instance.PlaySound(OnEnterClips);
        }
    }

    private IEnumerator ExecuteCommands() {

        while (enabled) {

            //while (commands.Count > 2) {
            //    print($"Overload dequed: {commands.Dequeue()}");

            //}

            if (commands.TryDequeue(out FeedbacksCommand command)) {

                if (command == FeedbacksCommand.PlayNormal) {
                    hoverFeedback.SetDirectionTopToBottom();
                    hoverFeedback.PlayFeedbacks();

                    print($"{card.GetCard().CardType}: Execute Play Normal");
                }
                else if (command == FeedbacksCommand.PlayReversed) {
                    hoverFeedback.SetDirectionBottomToTop();
                    hoverFeedback.PlayFeedbacks();

                    print($"{card.GetCard().CardType}: Execute Play Reversed");
                }
                else if (command == FeedbacksCommand.RevertToNormal) {
                    print($"{card.GetCard().CardType}: Execute Revert To Normal");

                    if (hoverFeedback.IsPlaying) {
                        hoverFeedback.Revert();
                        print($"Reverted to {hoverFeedback.Direction}");
                    }
                    else {
                        hoverFeedback.SetDirectionTopToBottom();
                        hoverFeedback.PlayFeedbacks();
                        print($"{card.GetCard().CardType}: Set direction to {hoverFeedback.Direction}");
                    }
                }
                else if (command == FeedbacksCommand.RevertToReversed) {
                    print($"{card.GetCard().CardType}: Execute Revert To Reversed");

                    if (hoverFeedback.IsPlaying) {
                        hoverFeedback.Revert();
                        print($"{card.GetCard().CardType}: Reverted to {hoverFeedback.Direction}");
                    }
                    else {
                        hoverFeedback.SetDirectionBottomToTop();
                        hoverFeedback.PlayFeedbacks();
                        print($"{card.GetCard().CardType}: Set direction to {hoverFeedback.Direction}");
                    }
                }

                float executeCooldown = 0.1f;
                yield return new WaitForSeconds(executeCooldown);
            }

            yield return null;
        }
    }
}

