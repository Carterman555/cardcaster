using UnityEngine;
using System;

public class TimedActionBehavior {
    private float actionTimer;
    private float actionCooldown;
    private Action onActionTriggered;
    private Func<bool> shouldStopCondition;
    private int actionsRemaining;

    public TimedActionBehavior(float actionCooldown, Action onActionTriggered, Func<bool> shouldStopCondition = null) {
        this.actionCooldown = actionCooldown;
        this.onActionTriggered = onActionTriggered;
        this.shouldStopCondition = shouldStopCondition;
        Stop();
    }

    public void Start(int totalActions = -1) {
        actionTimer = 0;
        actionsRemaining = totalActions;
    }

    public void Stop() {
        actionTimer = 0;
        actionsRemaining = 0;
    }

    public void UpdateLogic() {
        if (shouldStopCondition != null && shouldStopCondition()) {
            Stop();
            return;
        }

        if (actionsRemaining == 0) {
            Stop();
            return;
        }

        actionTimer += Time.deltaTime;
        if (actionTimer > actionCooldown) {
            onActionTriggered?.Invoke();
            actionTimer = 0;
            if (actionsRemaining > 0) {
                actionsRemaining--;
            }
        }
    }

    public void SetActionCooldown(float newCooldown) {
        actionCooldown = newCooldown;
    }

    public bool IsFinished() {
        return actionsRemaining == 0;
    }
}