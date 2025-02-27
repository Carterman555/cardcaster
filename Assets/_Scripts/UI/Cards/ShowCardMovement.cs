using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShowCardMovement;

public class ShowCardMovement : MonoBehaviour {


    public enum CommandType { MoveUp, MoveDown }

    #region Test

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

    private CommandType previousCommand;


    private void OnEnable() {
        //... starts in down pos
        previousCommand = CommandType.MoveDown;
    }

    // the moving up and down commands need to play the feedback if the feedback is not playing and revert it if it is playing.
    // they might need to set the direction, or I might be able to have the direction set from the "reverse direction on play" bool.
    // they need to store which pos the card is in by which command was played last (I need to make sure nothing external plays this
    // feedback because that would mess this up). I could have an enum of different states, but I think a bool and then the isPlaying
    // bool will work
    // I think I also might need to make sure there is a delay between commands (I will test this).

    public void MoveUp() {

        if (recordCommands) {
            RecordCommand(CommandType.MoveUp);
        }

        if (!enabled || previousCommand == CommandType.MoveUp) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.PlayFeedbacks();
        }

        previousCommand = CommandType.MoveUp;
    }

    public void MoveDown() {

        if (recordCommands) {
            RecordCommand(CommandType.MoveDown);
        }

        if (!enabled || previousCommand == CommandType.MoveDown) {
            return;
        }

        if (hoverFeedback.IsPlaying) {
            hoverFeedback.Revert();
        }
        else {
            hoverFeedback.PlayFeedbacks();
        }

        previousCommand = CommandType.MoveDown;
    }

}
