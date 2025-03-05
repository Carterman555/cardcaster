using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static ShowCardMovement;

public class ShowCardMovement : MonoBehaviour {

    [SerializeField] private MMF_Player hoverFeedback;

    public enum CommandType { None, MoveUp, MoveDown }
    private CommandType delayedCommand;

    private void OnEnable() {
        delayedCommand = CommandType.None;

        if (hoverFeedback.PlayCount > 0) {
            hoverFeedback.SetDirectionBottomToTop();
        }
    }

    [ContextMenu("Show")]
    public void Show() {

        if (!enabled) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            delayedCommand = CommandType.MoveUp;
            return;
        }

        if (hoverFeedback.InFirstState()) {
            hoverFeedback.PlayFeedbacks();
        }
    }

    [ContextMenu("Hide")]
    public void Hide() {

        if (!enabled) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            delayedCommand = CommandType.MoveDown;
            return;
        }

        if (hoverFeedback.Direction == MMFeedbacks.Directions.TopToBottom) {
            hoverFeedback.PlayFeedbacks();
        }
    }

    private void Update() {

        if (delayedCommand != CommandType.None) {
            if (!hoverFeedback.IsPlaying) {

                if (delayedCommand == CommandType.MoveUp) {
                    Show();
                }
                else if (delayedCommand == CommandType.MoveDown) {
                    Hide();
                }

                delayedCommand = CommandType.None;
            }
        }
    }
}
