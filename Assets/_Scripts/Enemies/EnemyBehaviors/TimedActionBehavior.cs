using UnityEngine;
using System;

public class TimedActionBehavior {
    private float actionTimer;
    private float actionCooldown;
    private Action onActionTriggered;
    private Func<bool> shouldStopCondition;

    public TimedActionBehavior(float actionCooldown, Action onActionTriggered, Func<bool> shouldStopCondition = null) {
        this.actionCooldown = actionCooldown;
        this.onActionTriggered = onActionTriggered;
        this.shouldStopCondition = shouldStopCondition;
        Stop();
    }

    public void Start() {
        actionTimer = 0;
    }

    public void Stop() {
        actionTimer = 0;
    }

    public void UpdateLogic() {
        if (shouldStopCondition != null && shouldStopCondition()) {
            Stop();
            return;
        }

        actionTimer += Time.deltaTime;
        if (actionTimer > actionCooldown) {
            onActionTriggered?.Invoke();
            actionTimer = 0;
        }
    }

    public void SetActionCooldown(float newCooldown) {
        actionCooldown = newCooldown;
    }
}
