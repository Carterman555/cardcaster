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

        if (hoverFeedback.PlayCount != 0) {
            hoverFeedback.SetDirectionBottomToTop();
        }
    }

    public void MoveUp() {

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

    public void MoveDown() {

        if (!enabled) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            delayedCommand = CommandType.MoveDown;
            return;
        }

        if (hoverFeedback.InSecondState()) {
            hoverFeedback.PlayFeedbacks();
        }
    }

    private void Update() {

        if (delayedCommand != CommandType.None) {
            if (!hoverFeedback.IsPlaying) {

                if (delayedCommand == CommandType.MoveUp) {
                    MoveUp();
                }
                else if (delayedCommand == CommandType.MoveDown) {
                    MoveDown();
                }

                delayedCommand = CommandType.None;
            }
        }
    }
}
