using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static ShowCardMovement;

public class ShowCardMovement : MonoBehaviour {

    #region Test (delete)

    [SerializeField] private Command[] testCommandSequence;
    [SerializeField] private List<Command> recordedCommandSequence = new();

    [SerializeField] private bool recordCommands;
    private float previousCommandTime;

    [Serializable]
    public struct Command {
        public float DelayBefore;
        public CommandType CommandType;
    }


    [ContextMenu("Test Sequence")]
    private void StartTestSequence() {
        StartCoroutine(TestSequence());
    }

    private IEnumerator TestSequence() {

        yield return new WaitForSeconds(1f);

        foreach (var command in testCommandSequence) {
            yield return new WaitForSeconds(command.DelayBefore);

            if (command.CommandType == CommandType.MoveUp) {
                MoveUp();
            }
            else if (command.CommandType == CommandType.MoveDown) {
                MoveDown();
            }
        }
    }

    private void RecordCommand(CommandType commandType) {

        float delayBefore = Time.time - previousCommandTime;

        Command command = new() {
            DelayBefore = delayBefore,
            CommandType = commandType
        };

        recordedCommandSequence.Add(command);

        previousCommandTime = Time.time;
    }

    [ContextMenu("Set Recorded")]
    private void SetRecordedCommands() {
        testCommandSequence = recordedCommandSequence.ToArray();
        recordedCommandSequence.Clear();
    }

    #endregion


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

        if (recordCommands) {
            RecordCommand(CommandType.MoveUp);
        }

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

        if (recordCommands) {
            RecordCommand(CommandType.MoveDown);
        }

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
