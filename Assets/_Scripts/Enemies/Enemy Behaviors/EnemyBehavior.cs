using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I might just make this GameBehavior in the future and cards can use them also
public class EnemyBehavior {

    protected Enemy enemy;

    private bool stopped = true;

    public virtual void Initialize(Enemy enemy) {
        this.enemy = enemy;
    }

    public virtual void FrameUpdateLogic() { }

    public virtual void PhysicsUpdateLogic() { }

    public virtual void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType) { }

    public virtual void OnDisable() { }

    public virtual void Start() {
        stopped = false;
    }

    public virtual void Stop() {
        stopped = true;
    }

    public bool IsStopped() {
        return stopped;
    }
}
